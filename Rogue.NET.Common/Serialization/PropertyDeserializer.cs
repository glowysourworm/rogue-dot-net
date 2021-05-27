using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rogue.NET.Common.Serialization
{
    internal class PropertyDeserializer
    {
        // Collection of formatters for serialization
        Dictionary<Type, IBaseFormatter> _formatters;

        // Collection of all types from loaded ASSEMBLIES with their EXPECTED HASH CODES
        Dictionary<int, HashedType> _loadedTypes;

        // Creates wrapped objects for deserialization
        DeserializationObjectFactory _factory;
        DeserializationResolver _resolver;

        internal PropertyDeserializer()
        {
            _formatters = new Dictionary<Type, IBaseFormatter>();
            _loadedTypes = new Dictionary<int, HashedType>();
            _factory = new DeserializationObjectFactory();
            _resolver = new DeserializationResolver();
        }

        internal T Deserialize<T>(Stream stream)
        {
            // Read root
            var wrappedRoot = ReadNext(stream, typeof(T));

            if (wrappedRoot.Reference.Type.Resolve() != typeof(T))
                throw new Exception("File type doesn't match the type of object being Deserialized:  " + wrappedRoot.Reference.Type.TypeName);

            // Create root node
            var node = CreateNode(wrappedRoot, PropertyDefinition.Empty);

            // Read stream recursively
            ReadRecurse(stream, node);

            // Stitch together object references recursively
            var resolvedRoot = _resolver.Resolve(node);

            // And -> We've -> Resolved() -> TheObject! :)
            return (T)resolvedRoot.NodeObject.Resolve().TheObject;
        }

        /// <summary>
        /// Creates a node based on a wrapped object along with the property definition. This must be read from the PropertyPlanner
        /// </summary>
        private DeserializationNodeBase CreateNode(DeserializationObjectBase wrappedObject, PropertyDefinition definition)
        {
            if (wrappedObject is DeserializationNullReference)
                return new DeserializationNode(wrappedObject, definition);

            else if (wrappedObject is DeserializationPrimitive)
                return new DeserializationNode(wrappedObject, definition);

            else if (wrappedObject is DeserializationValue)
                return new DeserializationNode(wrappedObject, definition);

            else if (wrappedObject is DeserializationObject)
                return new DeserializationNode(wrappedObject, definition);

            else if (wrappedObject is DeserializationCollection)
                return new DeserializationCollectionNode(wrappedObject as DeserializationCollection, definition);

            else
                throw new Exception("Unhandled DeserializationObjectBase type:  " + wrappedObject.GetType().ToString());
        }

        private void ReadRecurse(Stream stream, DeserializationNodeBase node)
        {
            // FROM SERIALIZER:
            //
            // Recursively identify node children and analyze by type inspection for SerializationObjectBase.
            // Select formatter for objects that are value types if no ctor / get methods are supplied
            //

            // STORE REFERENCE (DO THIS AT A LATER TIME WHEN OBJECT IS READY!)
            // _deserializedObjectcs.Add(node.NodeObject.Reference, node.NodeObject);

            // COLLECTION
            if (node is DeserializationCollectionNode)
            {
                var collectionNode = node as DeserializationCollectionNode;

                // HAND CONTROL TO FRONT END FOR CUSTOM PROPERTIES
                var definitions = collectionNode.NodeObject.GetPropertyDefinitions();

                // RECURSE ANY CUSTOM PROPERTIES
                foreach (var definition in definitions)
                {
                    // READ NEXT
                    var wrappedObject = ReadNext(stream, definition.PropertyType);

                    // VALIDATE THE PROPERTY!
                    if (wrappedObject.Reference.Type.Resolve() != definition.PropertyType)
                        throw new Exception("Invalid property for type:  " + definition.PropertyType + ", " + definition.PropertyName);

                    // Create node for the property
                    var subNode = CreateNode(wrappedObject, definition);

                    // Store sub-node
                    collectionNode.SubNodes.Add(subNode);

                    // RECURSE
                    ReadRecurse(stream, subNode);
                }

                for (int index = 0; index < collectionNode.Count; index++)
                {
                    // READ NEXT
                    var wrappedObject = ReadNext(stream, (collectionNode.NodeObject as DeserializationCollection).ElementType);

                    // Create child node
                    var childNode = CreateNode(wrappedObject, PropertyDefinition.CollectionElement);

                    // STORE CHILD NODE
                    collectionNode.Children.Add(childNode);

                    // RECURSE
                    ReadRecurse(stream, childNode);
                }
            }
            // NODE
            else if (node is DeserializationNode)
            {
                var nextNode = node as DeserializationNode;

                // NULL (Halt Recursion)
                if (nextNode.NodeObject is DeserializationNullReference)
                    return;

                // PRIMITIVE (Halt Recursion)
                else if (nextNode.NodeObject is DeserializationPrimitive)
                    return;

                // HAND CONTROL TO EITHER FRONT END OR GET REFLECTED PROPERTIES
                var definitions = nextNode.NodeObject.GetPropertyDefinitions();

                // Loop properties:  Verify sub-nodes -> Recurse
                foreach (var definition in definitions)
                {
                    // READ NEXT
                    var wrappedSubObject = ReadNext(stream, definition.PropertyType);

                    // VALIDATE THE PROPERTY!
                    if (wrappedSubObject.Reference.Type.Resolve() != definition.PropertyType)
                        throw new Exception("Invalid property for type:  " + definition.PropertyType + ", " + definition.PropertyName);

                    // Create sub-node
                    var subNode = CreateNode(wrappedSubObject, definition);

                    // Store sub-node
                    nextNode.SubNodes.Add(subNode);

                    // RECURSE
                    ReadRecurse(stream, subNode);
                }
            }
            else
                throw new Exception("Unhandled DeserializationNodeBase type:  PropertyDeserializer.DeserializeRecurse");
        }

        private DeserializationObjectBase ReadNext(Stream stream, Type expectedType)
        {
            // FROM SERIALIZER
            //
            // Serialize:  [ Null = 0,       Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Primitive = 1,  Serialization Mode, Hashed Type Code, Primitive Value ]
            // Serialize:  [ Value = 2,      Serialization Mode, Hashed Type Code, Hash Object Info Code ] (Recruse Sub - graph)
            // Serialize:  [ Object = 3,     Serialization Mode, Hashed Type Code, Hash Object Info Code ] (Recruse Sub - graph)
            // Serialize:  [ Reference = 4,  Serialization Mode, Hashed Type Code, Hash Object Info Code ]
            // Serialize:  [ Collection = 5, Serialization Mode, Hashed Type Code, Hash Object Info Code,
            //                               Child Count,
            //                               Collection Interface Type ] (loop) Children (Recruse Sub-graphs)

            var nextNode = Read<SerializedNodeType>(stream);
            var nextMode = Read<SerializationMode>(stream);
            var nextHash = Read<int>(stream);

            // Resolves the expected type with the actual hash code and returns the wrapped type
            var hashedType = ResolveType(expectedType, nextHash);

            switch (nextNode)
            {
                case SerializedNodeType.Null:
                    return _factory.CreateNullReference(new HashedObjectReference(hashedType), nextMode);
                case SerializedNodeType.Primitive:
                    {
                        // READ PRIMITIVE VALUE FROM STREAM
                        var primitive = Read(stream, hashedType.Resolve());

                        return _factory.CreatePrimitive(new HashedObjectInfo(primitive, hashedType.Resolve()), nextMode);
                    }
                case SerializedNodeType.Value:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        return _factory.CreateValue(new HashedObjectReference(hashedType, hashCode), nextMode);
                    }
                case SerializedNodeType.Object:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        return _factory.CreateObject(new HashedObjectReference(hashedType, hashCode), nextMode);
                    }

                case SerializedNodeType.Reference:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        return _factory.CreateObject(new HashedObjectReference(hashedType, hashCode), nextMode);
                    }
                case SerializedNodeType.Collection:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        // READ CHILD COUNT
                        var childCount = Read<int>(stream);

                        // READ INTERFACE TYPE
                        var interfaceType = Read<CollectionInterfaceType>(stream);

                        return _factory.CreateCollection(new HashedObjectReference(hashedType, hashCode), childCount, interfaceType, nextMode);
                    }
                default:
                    throw new Exception("Unhandled SerializedNodeType:  DeserializationObjectFactory.TypeTrack");
            }
        }

        private HashedType ResolveType(Type expectedType, int hashedTypeHashCode)
        {
            if (_loadedTypes.ContainsKey(hashedTypeHashCode))
                return _loadedTypes[hashedTypeHashCode];

            var hashedType = new HashedType(expectedType);

            if (hashedType.GetHashCode() != hashedTypeHashCode)
                throw new Exception("Invalid hash code read from stream for type:  " + expectedType.Name);

            _loadedTypes.Add(hashedType.GetHashCode(), hashedType);

            return hashedType;
        }

        private object Read(Stream stream, Type type)
        {
            var formatter = SelectFormatter(type);

            return formatter.Read(stream);
        }

        private T Read<T>(Stream stream)
        {
            var formatter = SelectFormatter(typeof(T));

            return (T)formatter.Read(stream);
        }

        private IBaseFormatter SelectFormatter(Type type)
        {
            if (!_formatters.ContainsKey(type))
                _formatters.Add(type, FormatterFactory.CreateFormatter(type));

            return _formatters[type];
        }
    }
}

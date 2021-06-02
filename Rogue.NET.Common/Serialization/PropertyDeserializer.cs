using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogue.NET.Common.Serialization
{
    internal class PropertyDeserializer
    {
        // Collection of formatters for serialization
        Dictionary<Type, IBaseFormatter> _primitiveFormatters;

        // Created for loading hashed TYPE TABLE
        HashedTypeFormatter _hashedTypeFormatter;

        // Collection of all types from loaded ASSEMBLIES with their EXPECTED HASH CODES
        Dictionary<int, HashedType> _loadedTypes;

        // Collection of SPECIFIED MODE properties BY HASHED TYPE (HASH CODE)
        Dictionary<int, IEnumerable<PropertyDefinition>> _specifiedObjectDict;

        // TYPE TABLE
        IList<HashedType> _typeTable;

        // Creates wrapped objects for deserialization
        DeserializationObjectFactory _factory;
        DeserializationResolver _resolver;

        internal PropertyDeserializer()
        {
            _primitiveFormatters = new Dictionary<Type, IBaseFormatter>();
            _loadedTypes = new Dictionary<int, HashedType>();
            _specifiedObjectDict = new Dictionary<int, IEnumerable<PropertyDefinition>>();
            _factory = new DeserializationObjectFactory();
            _resolver = new DeserializationResolver();
            _hashedTypeFormatter = new HashedTypeFormatter();
            _typeTable = new List<HashedType>();
        }

        internal IEnumerable<DeserializationObjectBase> GetDeserializedObjects()
        {
            return _resolver.GetDeserializedObjects().Values;
        }

        internal IEnumerable<HashedType> GetTypeTable()
        {
            return _typeTable;
        }

        internal T Deserialize<T>(Stream stream)
        {
            // Procedure
            //
            // 1) Read TYPE TABLE
            // 2) Store type table in LOADED TYPES
            // 3) Read the SPECIFIED MODE TABLE
            // 4) Read Object from stream
            // 5) Wrap into node for deserializing
            // 6) Recure on the node
            // 7) Resolve the object graph
            //

            // Read TYPE TABLE
            var typeTableCount = Read<int>(stream);

            for (int index = 0; index < typeTableCount; index++)
            {
                // READ HASHED TYPE (Recurses)
                var hashedType = _hashedTypeFormatter.Read(stream);

                // LOAD TYPE TABLE - CREATES UNIQUE HASH CODE
                _loadedTypes.Add(hashedType.GetHashCode(), hashedType);

                // Store for the manifest
                _typeTable.Add(hashedType);
            }

            // SPECIFIED MODE TABLE
            var specifiedObjectCount = Read<int>(stream);

            for (int index = 0; index < specifiedObjectCount; index++)
            {
                var properties = new List<PropertyDefinition>();

                // HASH CODE - AS REFERENCE WHEN DESERIALIZING
                var typeHashCode = Read<int>(stream);

                // PROPERTY COUNT
                var propertyCount = Read<int>(stream);

                for (int propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                {
                    // PROPERTY NAME
                    var propertyName = Read<string>(stream);

                    // PROPERTY DECLARING TYPE - AS HASHED TYPE!!!
                    var propertyType = _hashedTypeFormatter.Read(stream);

                    properties.Add(new PropertyDefinition()
                    {
                        PropertyName = propertyName,
                        PropertyType = propertyType.GetDeclaringType(),
                        IsUserDefined = true
                    });
                }

                // SPECIFIED MODE TYPE IS READY!
                _specifiedObjectDict.Add(typeHashCode, properties);
            }

            // Read root
            var wrappedRoot = ReadNext(stream, typeof(T));

            if (wrappedRoot.Reference.Type.GetImplementingType() != typeof(T))
                throw new Exception("File type doesn't match the type of object being Deserialized:  " + wrappedRoot.Reference.Type.DeclaringType);

            // Create root node
            var node = CreateNode(wrappedRoot, PropertyDefinition.Empty);

            // Read stream recursively
            ReadRecurse(stream, node);

            // Stitch together object references recursively
            var resolvedRoot = _resolver.Resolve(node);

            // And -> We've -> Resolved() -> TheObject()! :)
            return (T)resolvedRoot.NodeObject.Resolve().GetObject();
        }

        /// <summary>
        /// Creates a node based on a wrapped object along with the property definition. This must be read from the PropertyPlanner
        /// </summary>
        private DeserializationNodeBase CreateNode(DeserializationObjectBase wrappedObject, PropertyDefinition definition)
        {
            if (wrappedObject is DeserializationNullPrimitive)
                return new DeserializationNode(wrappedObject, definition);

            else if (wrappedObject is DeserializationNullReference)
                return new DeserializationNode(wrappedObject, definition);

            else if (wrappedObject is DeserializationPrimitive)
                return new DeserializationNode(wrappedObject, definition);

            else if (wrappedObject is DeserializationReference)
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

            // COLLECTION
            if (node is DeserializationCollectionNode)
            {
                var collectionNode = node as DeserializationCollectionNode;

                // Fetch definitions for properties from the node object
                var definitions = collectionNode.NodeObject.GetPropertyDefinitions();

                // RECURSE ANY CUSTOM PROPERTIES
                foreach (var definition in definitions)
                {
                    // READ NEXT
                    var wrappedObject = ReadNext(stream, definition.PropertyType);

                    // NOTE*** Type Discrepancies:  Have to patch these from the TYPE TABLE for when the NON user defined
                    //                              properties have a different implementing type
                    //

                    // VALIDATE THE PROPERTY!
                    if (!definition.IsUserDefined)
                    {
                        if (wrappedObject.Reference.Type.GetImplementingType() != definition.PropertyType &&
                            wrappedObject.Reference.Type.GetDeclaringType() != definition.PropertyType)
                        {
                            throw new Exception("Invalid property for type:  " + definition.PropertyType + ", " + definition.PropertyName);
                        }
                        // PATCH PROPERTY DEFINITION FOR TYPE DISCREPANCIES
                        else if (wrappedObject.Reference.Type.GetImplementingType() != definition.PropertyType)
                        {
                            definition.PropertyType = wrappedObject.Reference.Type.GetImplementingType();
                        }
                    }

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

                // PRIMITIVE NULL (Halt Recursion)
                if (nextNode.NodeObject is DeserializationNullPrimitive)
                    return;

                // NULL (Halt Recursion)
                else if (nextNode.NodeObject is DeserializationNullReference)
                    return;

                // PRIMITIVE (Halt Recursion)
                else if (nextNode.NodeObject is DeserializationPrimitive)
                    return;

                // REFERENCE (Halt Recursion)
                else if (nextNode.NodeObject is DeserializationReference)
                    return;

                // HAND CONTROL TO EITHER FRONT END OR GET REFLECTED PROPERTIES
                var definitions = nextNode.NodeObject.GetPropertyDefinitions();

                // Loop properties:  Verify sub-nodes -> Recurse
                foreach (var definition in definitions)
                {
                    // READ NEXT
                    var wrappedSubObject = ReadNext(stream, definition.PropertyType);

                    // NOTE*** Type Discrepancies:  Have to patch these from the TYPE TABLE for when the NON user defined
                    //                              properties have a different implementing type
                    //

                    // VALIDATE THE PROPERTY!
                    if (!definition.IsUserDefined)
                    {
                        if (wrappedSubObject.Reference.Type.GetImplementingType() != definition.PropertyType &&
                            wrappedSubObject.Reference.Type.GetDeclaringType() != definition.PropertyType)
                        { 
                            throw new Exception("Invalid property for type:  " + definition.PropertyType + ", " + definition.PropertyName);
                        }
                        // PATCH PROPERTY DEFINITION FOR TYPE DISCREPANCIES
                        else if (wrappedSubObject.Reference.Type.GetImplementingType() != definition.PropertyType)
                        {
                            definition.PropertyType = wrappedSubObject.Reference.Type.GetImplementingType();
                        }
                    }

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
            // Serialize:  [ Null Primitive = 0, Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Null = 1,           Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Primitive = 2,      Serialization Mode, Hashed Type Code, Primitive Value ]
            // Serialize:  [ Value = 3,          Serialization Mode, Hashed Type Code, Hash Object Info Code ] (Recruse Sub - graph)
            // Serialize:  [ Object = 4,         Serialization Mode, Hashed Type Code, Hash Object Info Code ] (Recruse Sub - graph)
            // Serialize:  [ Reference = 5,      Serialization Mode, Hashed Type Code, Hash Object Info Code ]
            // Serialize:  [ Collection = 6,     Serialization Mode, Hashed Type Code, Hash Object Info Code,
            //                                   Child Count,
            //                                   Collection Interface Type ] (loop) Children (Recruse Sub-graphs)

            var nextNode = Read<SerializedNodeType>(stream);
            var nextMode = Read<SerializationMode>(stream);
            var nextHash = Read<int>(stream);

            // Resolves the expected type with the actual hash code and returns the wrapped type
            var hashedType = ResolveType(expectedType, nextHash);

            switch (nextNode)
            {
                case SerializedNodeType.NullPrimitive:
                    return _factory.CreateNullPrimitive(new HashedObjectReference(hashedType), nextMode);
                case SerializedNodeType.Null:
                    return _factory.CreateNullReference(new HashedObjectReference(hashedType), nextMode);
                case SerializedNodeType.Primitive:
                    {
                        // READ PRIMITIVE VALUE FROM STREAM
                        var primitive = Read(stream, hashedType.GetImplementingType());

                        return _factory.CreatePrimitive(new HashedObjectInfo(primitive, hashedType.GetImplementingType()), nextMode);
                    }
                case SerializedNodeType.Value:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        // GET PROPERTY DEFINITIONS
                        var definitions = GetPropertyDefinitions(hashedType, nextMode);

                        return _factory.CreateValue(new HashedObjectReference(hashedType, hashCode), nextMode, definitions);
                    }
                case SerializedNodeType.Object:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        // GET PROPERTY DEFINITIONS
                        var definitions = GetPropertyDefinitions(hashedType, nextMode);

                        return _factory.CreateObject(new HashedObjectReference(hashedType, hashCode), nextMode, definitions);
                    }

                case SerializedNodeType.Reference:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        return _factory.CreateReference(new HashedObjectReference(hashedType, hashCode), nextMode);
                    }
                case SerializedNodeType.Collection:
                    {
                        // READ REFERENCE HASH FROM STREAM
                        var hashCode = Read<int>(stream);

                        // READ CHILD COUNT
                        var childCount = Read<int>(stream);

                        // READ INTERFACE TYPE
                        var interfaceType = Read<CollectionInterfaceType>(stream);

                        // GET PROPERTY DEFINITIONS - SPECIFIED MODE ONLY!!!
                        var definitions = nextMode == SerializationMode.Specified ? GetPropertyDefinitions(hashedType, nextMode) :
                                                                                    new PropertyDefinition[] { };

                        return _factory.CreateCollection(new HashedObjectReference(hashedType, hashCode), childCount, interfaceType, nextMode, definitions);
                    }
                default:
                    throw new Exception("Unhandled SerializedNodeType:  DeserializationObjectFactory.TypeTrack");
            }
        }

        private IEnumerable<PropertyDefinition> GetPropertyDefinitions(HashedType hashedType, SerializationMode mode)
        {
            switch (mode)
            {
                case SerializationMode.Default:
                    {
                        var propertyInfos = RecursiveSerializerStore.GetOrderedProperties(hashedType.GetImplementingType());

                        return propertyInfos.Select(info => new PropertyDefinition()
                        {
                            PropertyName = info.Name,
                            PropertyType = info.PropertyType,
                            IsUserDefined = false
                        });
                    }
                case SerializationMode.Specified:
                    {
                        if (!_specifiedObjectDict.ContainsKey(hashedType.GetHashCode()))
                            throw new Exception("SPECIFIED MODE property definitions not found:  " + hashedType.ToString());

                        return _specifiedObjectDict[hashedType.GetHashCode()];
                    }
                case SerializationMode.None:
                default:
                    throw new Exception("Unhandled SerializationMode Type:  PropertyDeserializer.GetPropertyDefinitions");
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
            if (!_primitiveFormatters.ContainsKey(type))
                _primitiveFormatters.Add(type, FormatterFactory.CreatePrimitiveFormatter(type));

            return _primitiveFormatters[type];
        }
    }
}

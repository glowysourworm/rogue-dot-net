using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class PropertyDeserializer
    {
        // Collection of formatters for serialization
        SimpleDictionary<Type, IBaseFormatter> _primitiveFormatters;

        // Created for loading hashed TYPE TABLE
        HashedTypeFormatter _hashedTypeFormatter;

        // Collection of all types from loaded ASSEMBLIES with their EXPECTED HASH CODES
        SimpleDictionary<int, HashedType> _loadedTypes;

        // Collection of property specifications by OBJECT ID
        SimpleDictionary<int, PropertySpecification> _specifiedPropertyDict;

        // Output manifest
        IList<SerializedNodeManifest> _outputManifest;

        // Creates wrapped objects for deserialization
        DeserializationObjectFactory _factory;
        DeserializationResolver _resolver;

        internal PropertyDeserializer()
        {
            _primitiveFormatters = new SimpleDictionary<Type, IBaseFormatter>();
            _loadedTypes = new SimpleDictionary<int, HashedType>();
            _specifiedPropertyDict = new SimpleDictionary<int, PropertySpecification>();
            _factory = new DeserializationObjectFactory();
            _resolver = new DeserializationResolver();
            _hashedTypeFormatter = new HashedTypeFormatter();
            _outputManifest = new List<SerializedNodeManifest>();
        }

        internal IEnumerable<DeserializationObjectBase> GetDeserializedObjects()
        {
            return _resolver.GetDeserializedObjects().Values;
        }

        internal IEnumerable<HashedType> GetTypeTable()
        {
            return _loadedTypes.Values;
        }

        internal IList<SerializedNodeManifest> GetManifest()
        {
            return _outputManifest;
        }

        internal T Deserialize<T>(Stream stream)
        {
            // Procedure
            //
            // 1) Read TYPE TABLE
            // 2) Store type table in LOADED TYPES
            // 3) Read the PROPERTY TABLE (Property Specification -> int[] of Object Id's)
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

                // LOAD TYPE TABLE - UNIQUE HASH CODE
                _loadedTypes.Add(hashedType.GetHashCode(), hashedType);
            }

            // PROPERTY TABLE
            var propertyTableCount = Read<int>(stream);

            for (int index = 0; index < propertyTableCount; index++)
            {
                // PROPERTY SPECIFICATION { Count, Hashed Type, PropertyDefinition[] }
                var propertyDefinitionCount = Read<int>(stream);

                // PROPERTY SPECIFICATION TYPE
                var typeHashCode = Read<int>(stream);

                // VALIDATE HASHED TYPE
                if (!_loadedTypes.ContainsKey(typeHashCode))
                    throw new Exception("Missing HashedType for PropertySpecification");

                var objectType = _loadedTypes[typeHashCode];

                // REFLECTION SUPPORT -> CHECK FOR CHANGE TO SPECIFICATION
                var currentSpecification = RecursiveSerializerStore.GetOrderedProperties(objectType);

                var properties = new List<PropertyDefinition>();

                for (int propertyIndex = 0; propertyIndex < propertyDefinitionCount; propertyIndex++)
                {
                    // PROPERTY NAME
                    var propertyName = Read<string>(stream);

                    // PROPERTY TYPE
                    var propertyType = _hashedTypeFormatter.Read(stream);

                    // IS USER DEFINED -> NO REFLECTION SUPPORT!
                    var isUserDefined = Read<bool>(stream);

                    if (isUserDefined)
                        properties.Add(new PropertyDefinition(null)
                        {
                            IsUserDefined = true,
                            PropertyName = propertyName,
                            PropertyType = propertyType
                        });

                    // NOT USER DEFINED -> REFLECTION SUPPORT
                    else
                    {
                        // Check ASSIGNABILITY of the specification property against the stored HASHED TYPE
                        var propertyDefinition = currentSpecification.Definitions
                                                                     .FirstOrDefault(definition =>
                                                                     {
                                                                         return definition.PropertyName.Equals(propertyName) &&
                                                                                definition.PropertyType.GetDeclaringType()
                                                                                                       .IsAssignableFrom(propertyType.GetImplementingType());
                                                                     });


                        // PROPERTY REMOVED FROM SPECIFICATION!!
                        if (propertyDefinition == null)
                            throw new RecursiveSerializerException(objectType, string.Format("Property Removed from type {0}. To read this data - must use IgnoreRemovals option", objectType));

                        // REFLECTION SUPPORT AVAILABLE!
                        properties.Add(new PropertyDefinition(propertyDefinition.GetReflectedInfo())
                        {
                            IsUserDefined = false,
                            PropertyName = propertyName,
                            PropertyType = propertyType     // OUR PROPERTY TYPE
                        });
                    }
                }

                var specification = new PropertySpecification(objectType, properties);

                // OBJECT REFERENCES
                var objectIdCount = Read<int>(stream);

                for (int objectIndex = 0; objectIndex < objectIdCount; objectIndex++)
                {
                    // OBJECT ID
                    var objectId = Read<int>(stream);

                    if (_specifiedPropertyDict.ContainsKey(objectId))
                        throw new Exception("Duplicate OBJECT ID found in property specification table");

                    _specifiedPropertyDict.Add(objectId, specification);
                }
            }

            // Read root
            var wrappedRoot = ReadNext(stream, new HashedType(typeof(T)));

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
                var specification = collectionNode.NodeObject.GetPropertySpecification();

                // RECURSE ANY CUSTOM PROPERTIES
                foreach (var definition in specification.Definitions)
                {
                    // READ NEXT
                    var wrappedObject = ReadNext(stream, definition.PropertyType);

                    // Create node for the property
                    var subNode = CreateNode(wrappedObject, definition);

                    // Store sub-node
                    collectionNode.SubNodes.Add(subNode);

                    // RECURSE
                    ReadRecurse(stream, subNode);
                }

                // Iterate expected ELEMENT TYPES
                var collection = (collectionNode.NodeObject as DeserializationCollection);

                for (int index = 0; index < collection.Count; index++)
                {
                    // Expected type serialized with the stream
                    var expectedChildType = collection.ElementTypes[index];

                    // READ NEXT
                    var wrappedObject = ReadNext(stream, expectedChildType);

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

                var specification = nextNode.NodeObject.GetPropertySpecification();

                // Loop properties:  Verify sub-nodes -> Recurse
                foreach (var definition in specification.Definitions)
                {
                    // READ NEXT
                    var wrappedSubObject = ReadNext(stream, definition.PropertyType);

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

        private DeserializationObjectBase ReadNext(Stream stream, HashedType expectedType)
        {
            // FROM SERIALIZER
            //
            // Serialize:  [ Null Primitive = 0, Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Null = 1,           Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Primitive = 2,      Serialization Mode, Hashed Type Code, Primitive Value ]
            // Serialize:  [ Value = 3,          Serialization Mode, Hashed Type Code, Object Id ] (Recruse Sub - graph)
            // Serialize:  [ Object = 4,         Serialization Mode, Hashed Type Code, Object Id ] (Recruse Sub - graph)
            // Serialize:  [ Reference = 5,      Serialization Mode, Hashed Type Code, Object Id ]
            // Serialize:  [ Collection = 6,     Serialization Mode, Hashed Type Code, Object Id,
            //                                   Collection Interface Type,
            //                                   Child Count,
            //                                   Child Hash Type Code[] ] (loop) Children (Recruse Sub-graphs)

            var manifest = new SerializedNodeManifest();

            var nextNode = Read<SerializedNodeType>(stream);
            var nextMode = Read<SerializationMode>(stream);
            var nextTypeHash = Read<int>(stream);

            // VAILDATE HASHED TYPE
            if (nextTypeHash != expectedType.GetHashCode())
                throw new Exception("Invalid hash code read from stream for EXPECTED type:  " + expectedType.ToString());

            // Write manifest
            manifest.Assembly = expectedType.DeclaringAssembly;
            manifest.GenericArgumentTypes = expectedType.DeclaringGenericArguments.Transform(hashedType => hashedType.ToString());
            manifest.IsGeneric = expectedType.DeclaringIsGeneric;
            manifest.Mode = nextMode;
            manifest.Node = nextNode;
            manifest.NodeTypeHashCode = nextTypeHash;
            manifest.Type = expectedType.ToString();

            DeserializationObjectBase result = null;

            switch (nextNode)
            {
                case SerializedNodeType.NullPrimitive:
                    result = _factory.CreateNullPrimitive(new ObjectReference(expectedType, 0), nextMode);
                    break;
                case SerializedNodeType.Null:
                    result = _factory.CreateNullReference(new ObjectReference(expectedType, 0), nextMode);
                    break;
                case SerializedNodeType.Primitive:
                    {
                        // READ PRIMITIVE VALUE FROM STREAM
                        var primitive = Read(stream, expectedType.GetImplementingType());

                        result = _factory.CreatePrimitive(new ObjectInfo(primitive, expectedType), nextMode);

                        break;
                    }
                case SerializedNodeType.Value:
                    {
                        // READ OBJECT ID FROM STREAM
                        var objectId = Read<int>(stream);

                        // Create reference
                        var reference = new ObjectReference(expectedType, objectId);

                        // GET PROPERTY SPECIFICATION
                        var specification = _specifiedPropertyDict[objectId];

                        result = _factory.CreateValue(reference, nextMode, specification);

                        break;
                    }
                case SerializedNodeType.Object:
                    {
                        // READ OBJECT ID FROM STREAM
                        var objectId = Read<int>(stream);

                        // Create reference
                        var reference = new ObjectReference(expectedType, objectId);

                        // GET PROPERTY SPECIFICATION
                        var specification = _specifiedPropertyDict[objectId];

                        result = _factory.CreateObject(reference, nextMode, specification);

                        break;
                    }

                case SerializedNodeType.Reference:
                    {
                        // READ OBJECT ID FROM STREAM
                        var objectId = Read<int>(stream);

                        result = _factory.CreateReference(new ObjectReference(expectedType, objectId), nextMode);

                        break;
                    }
                case SerializedNodeType.Collection:
                    {
                        // READ OBJECT ID FROM STREAM
                        var objectId = Read<int>(stream);

                        // READ INTERFACE TYPE
                        var interfaceType = Read<CollectionInterfaceType>(stream);

                        // READ CHILD COUNT
                        var childCount = Read<int>(stream);

                        // ELEMENT HASH TYPES
                        var elementTypes = new List<HashedType>(childCount);

                        for (int index = 0; index < childCount; index++)
                        {
                            var elementTypeCode = Read<int>(stream);

                            // VAILDATE HASHED TYPE
                            if (!_loadedTypes.ContainsKey(elementTypeCode))
                                throw new Exception("Invalid hash code read from stream for collection type:  " + expectedType.ToString());

                            elementTypes.Add(_loadedTypes[elementTypeCode]);
                        }

                        // Create reference
                        var reference = new ObjectReference(expectedType, objectId);

                        // GET PROPERTY SPECIFICATION
                        var specification = _specifiedPropertyDict[objectId];

                        result = _factory.CreateCollection(reference, interfaceType, childCount, nextMode, specification, elementTypes);

                        // Write manifest
                        manifest.CollectionCount = childCount;
                        manifest.CollectionType = interfaceType;

                        break;
                    }
                default:
                    throw new Exception("Unhandled SerializedNodeType:  DeserializationObjectFactory.TypeTrack");
            }

            // Write manifest
            manifest.ObjectId = result.Reference.ReferenceId;

            // MANIFEST NODE READY!
            _outputManifest.Add(manifest);

            return result;
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

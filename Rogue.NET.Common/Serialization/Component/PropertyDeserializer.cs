using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.IO.Interface;
using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class PropertyDeserializer
    {
        // Collection of all types from loaded ASSEMBLIES with their EXPECTED HASH CODES
        SimpleDictionary<int, HashedType> _loadedTypes;

        // Collection of property specifications by OBJECT ID
        SimpleDictionary<int, PropertySpecification> _specifiedPropertyDict;

        // Output manifest
        IList<SerializedNodeManifest> _outputManifest;

        // Creates wrapped objects for deserialization
        DeserializationObjectFactory _factory;
        DeserializationResolver _resolver;

        // PRIMARY INPUT STREAM
        ISerializationStreamReader _reader;

        readonly RecursiveSerializerConfiguration _configuration;

        internal PropertyDeserializer(RecursiveSerializerConfiguration configuration)
        {
            _loadedTypes = new SimpleDictionary<int, HashedType>();
            _specifiedPropertyDict = new SimpleDictionary<int, PropertySpecification>();
            _factory = new DeserializationObjectFactory();
            _resolver = new DeserializationResolver();
            _outputManifest = new List<SerializedNodeManifest>();
            _configuration = configuration;
        }

        internal IEnumerable<DeserializedNodeBase> GetDeserializedObjects()
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

        internal T Deserialize<T>(ISerializationStreamReader reader)
        {
            _reader = reader;

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
            var typeTableCount = _reader.Read<int>();

            for (int index = 0; index < typeTableCount; index++)
            {
                // READ HASHED TYPE (Recurses)
                var hashedType = _reader.Read<HashedType>();

                // LOAD TYPE TABLE - UNIQUE HASH CODE
                _loadedTypes.Add(hashedType.GetHashCode(), hashedType);
            }

            // PROPERTY TABLE
            var propertyTableCount = _reader.Read<int>();

            for (int index = 0; index < propertyTableCount; index++)
            {
                // PROPERTY SPECIFICATION { Count, Hashed Type, PropertyDefinition[] }
                var propertyDefinitionCount = _reader.Read<int>();

                // PROPERTY SPECIFICATION TYPE
                var typeHashCode = _reader.Read<int>();

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
                    var propertyName = _reader.Read<string>();

                    // PROPERTY TYPE (HASH CODE)
                    var propertyTypeHashCode = _reader.Read<int>();

                    if (!_loadedTypes.ContainsKey(propertyTypeHashCode))
                        throw new Exception("Missing HashedType for PropertyDefinition");

                    var propertyType = _loadedTypes[propertyTypeHashCode];

                    // IS USER DEFINED -> NO REFLECTION SUPPORT!
                    var isUserDefined = _reader.Read<bool>();

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
                var objectIdCount = _reader.Read<int>();

                for (int objectIndex = 0; objectIndex < objectIdCount; objectIndex++)
                {
                    // OBJECT ID
                    var objectId = _reader.Read<int>();

                    if (_specifiedPropertyDict.ContainsKey(objectId))
                        throw new Exception("Duplicate OBJECT ID found in property specification table");

                    _specifiedPropertyDict.Add(objectId, specification);
                }
            }

            // Read root
            var rootNode = ReadNext(PropertyDefinition.Empty, new HashedType(typeof(T)), false);

            if (rootNode.Type.GetImplementingType() != typeof(T))
                throw new RecursiveSerializerException(rootNode.Type, "File type doesn't match the type of object being Deserialized");

            // Read stream recursively
            ReadRecurse(rootNode);

            // Stitch together object references recursively
            var resolvedRoot = _resolver.Resolve(rootNode, _loadedTypes);

            // And -> We've -> Resolved() -> TheObject()! :)
            return (T)resolvedRoot.Resolve();
        }

        private void ReadRecurse(DeserializedNodeBase node)
        {
            // FROM SERIALIZER:
            //
            // Recursively identify node children and analyze by type inspection for SerializationObjectBase.
            // Select formatter for objects that are value types if no ctor / get methods are supplied
            //

            // COLLECTION
            if (node is DeserializedCollectionNode)
            {
                var collectionNode = node as DeserializedCollectionNode;

                // Fetch definitions for properties from the node object
                var specification = collectionNode.GetPropertySpecification();

                // RECURSE ANY CUSTOM PROPERTIES
                foreach (var definition in specification.Definitions)
                {
                    // READ NEXT
                    var propertyNode = ReadNext(definition, definition.PropertyType, false);

                    // Store sub-node
                    collectionNode.SubNodes.Add(propertyNode);

                    // RECURSE
                    ReadRecurse(propertyNode);
                }

                // Iterate expected ELEMENT TYPES
                var collection = (collectionNode as DeserializedCollectionNode);

                // VALIDATE ELEMENT TYPE
                if (!_loadedTypes.ContainsKey(collection.ElementType.GetHashCode()))
                    throw new Exception("Missing ElementType for collection: " + collection.Type.ToString());

                for (int index = 0; index < collection.Count; index++)
                {
                    // READ NEXT (ELEMENT TYPE => DECLARING TYPE)
                    var elementNode = ReadNext(PropertyDefinition.CollectionElement, collection.ElementType, true);

                    // STORE CHILD NODE
                    collectionNode.CollectionNodes.Add(elementNode);

                    // RECURSE
                    ReadRecurse(elementNode);
                }
            }

            // NODE
            else if (node is DeserializedObjectNode)
            {
                var nextNode = node as DeserializedObjectNode;

                var specification = nextNode.GetPropertySpecification();

                // Loop properties:  Verify sub-nodes -> Recurse
                foreach (var definition in specification.Definitions)
                {
                    // READ NEXT
                    var subNode = ReadNext(definition, definition.PropertyType, false);

                    // Store sub-node
                    nextNode.SubNodes.Add(subNode);

                    // RECURSE
                    ReadRecurse(subNode);
                }
            }

            // REFERENCE NODE -> Halt Recursion
            else if (node is DeserializedReferenceNode)
            {
                return;
            }

            // LEAF NODE -> Halt Recursion
            else if (node is DeserializedLeafNode)
            {
                return;
            }

            // NULL LEAF NODE -> Halt Recursion
            else if (node is DeserializedNullLeafNode)
            {
                return;
            }

            else
                throw new Exception("Unhandled DeserializedNodeBase type:  PropertyDeserializer.cs");
        }

        private DeserializedNodeBase ReadNext(PropertyDefinition definingProperty, HashedType expectedType, bool isCollectionElement)
        {
            // FROM SERIALIZER
            //
            // Serialize:  [ Null Primitive = 0, Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Null = 1,           Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Primitive = 2,      Serialization Mode, Hashed Type Code, Primitive Value ]
            // Serialize:  [ Object = 3,         Serialization Mode, Hashed Type Code, Object Id ] (Recruse Sub - graph)
            // Serialize:  [ Reference = 4,      Serialization Mode, Hashed Type Code, Reference Object Id ]
            // Serialize:  [ Collection = 5,     Serialization Mode, Hashed Type Code, Object Id,
            //                                   Collection Interface Type,
            //                                   Child Count,
            //                                   Child Hash Type Code ] (loop) Children (Recruse Sub-graphs)

            //var manifest = new SerializedNodeManifest();

            var nextNode = _reader.Read<SerializedNodeType>();
            var nextMode = _reader.Read<SerializationMode>();
            var nextTypeHash = _reader.Read<int>();

            HashedType resolvedType = expectedType;

            // VAILDATE HASHED TYPE
            if (nextTypeHash != expectedType.GetHashCode())
            {
                // CHECK THAT TYPE IS LOADED
                if (!_loadedTypes.ContainsKey(nextTypeHash))
                    throw new Exception("Invalid hash code read from stream for EXPECTED type:  " + expectedType.ToString());

                var actualType = _loadedTypes[nextTypeHash];

                // CHECK FOR COLLECTION ASSIGNABILITY
                if (isCollectionElement)
                {
                    if (!expectedType.GetDeclaringType().IsAssignableFrom(actualType.GetImplementingType()))
                        throw new Exception("Collection assignability failed for deserialized type:  " + expectedType.ToString());

                    // RESOLVE THE ACTUAL TYPE FOR THE COLLECTION
                    else
                        resolvedType = actualType;
                }
            }

            //// Write manifest
            //manifest.Mode = nextMode;
            //manifest.Node = nextNode;
            //manifest.Type = resolvedType.ToString();

            DeserializedNodeBase result = null;

            switch (nextNode)
            {
                case SerializedNodeType.NullPrimitive:
                    result = _factory.CreateNullPrimitive(definingProperty, resolvedType, nextMode);
                    break;
                case SerializedNodeType.Null:
                    result = _factory.CreateNullReference(definingProperty, resolvedType, nextMode);
                    break;
                case SerializedNodeType.Primitive:
                    {
                        // READ PRIMITIVE VALUE FROM STREAM
                        var primitive = _reader.Read(resolvedType.GetImplementingType());

                        result = _factory.CreatePrimitive(definingProperty, primitive, resolvedType, nextMode);

                        break;
                    }
                case SerializedNodeType.Object:
                    {
                        // READ OBJECT ID FROM STREAM
                        var objectId = _reader.Read<int>();

                        // GET PROPERTY SPECIFICATION
                        var specification = _specifiedPropertyDict[objectId];

                        result = _factory.CreateObject(definingProperty, objectId, resolvedType, nextMode, specification);

                        break;
                    }

                case SerializedNodeType.Reference:
                    {
                        // READ OBJECT ID FROM STREAM
                        var referenceId = _reader.Read<int>();

                        result = _factory.CreateReference(definingProperty, referenceId, resolvedType, nextMode);

                        break;
                    }
                case SerializedNodeType.Collection:
                    {
                        // READ OBJECT ID FROM STREAM
                        var objectId = _reader.Read<int>();

                        // READ INTERFACE TYPE
                        var interfaceType = _reader.Read<CollectionInterfaceType>();

                        // READ CHILD COUNT
                        var childCount = _reader.Read<int>();

                        // ELEMENT HASH TYPE CODE
                        var elementTypeHashCode = _reader.Read<int>();

                        if (!_loadedTypes.ContainsKey(elementTypeHashCode))
                            throw new Exception("Missing collection element type hash code " + resolvedType.ToString());

                        var elementType = _loadedTypes[elementTypeHashCode];

                        // GET PROPERTY SPECIFICATION
                        var specification = _specifiedPropertyDict[objectId];

                        result = _factory.CreateCollection(definingProperty, objectId, resolvedType, interfaceType, childCount, nextMode, specification, elementType);

                        // Write manifest
                        //manifest.CollectionCount = childCount;
                        //manifest.CollectionType = interfaceType;

                        break;
                    }
                default:
                    throw new Exception("Unhandled SerializedNodeType:  DeserializationObjectFactory.TypeTrack");
            }

            // Write manifest
            //manifest.ObjectId = result.Reference.ReferenceId;

            // MANIFEST NODE READY!
            //_outputManifest.Add(manifest);

            return result;
        }
    }
}

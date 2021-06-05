using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.IO.Interface;
using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class PropertySerializer
    {
        // Collection of UNIQUE objects that HAVE BEEN SERIALIZED
        SimpleDictionary<ObjectInfo, SerializationObjectBase> _serializedObjects;

        // ORDERED NODE MANIFEST
        IList<SerializedNodeManifest> _outputManifest;

        // SINGLE INSTANCE - ONE PER RUN
        ObjectInfoResolver _resolver;

        // PRIMARY OUTPUT STREAM
        ISerializationStreamWriter _writer;

        internal PropertySerializer()
        {
            _serializedObjects = new SimpleDictionary<ObjectInfo, SerializationObjectBase>();
            _outputManifest = new List<SerializedNodeManifest>();
            _resolver = new ObjectInfoResolver();
        }

        internal void Serialize<T>(ISerializationStreamWriter writer, T theObject)
        {
            _writer = writer;

            // SINGLE INSTANCE PER RUN - STORES ALL RESOLVED TYPES
            _resolver = new ObjectInfoResolver();

            var planner = new SerializationPlanner<T>(_resolver);

            // Procedure
            //
            // 0) Reset the Id counter
            // 1) Run the planner to create reference dictionary and node tree
            // 2) STORE TYPE TABLE
            // 3) STORE PROPERTY TABLE (Property Specification -> int[] of Object Id's)
            // 4) (Recurse) Serialize the node graph OBJECTS
            // 5) Validate OUR serialized objects against the ISerializationPlan
            //

            // Reset Id Counter
            ObjectInfo.ResetCounter();

            // Run the planner
            var plan = planner.Plan(theObject);

            // TYPE TABLE COUNT
            _writer.Write<int>(_resolver.GetResolvedTypes().Count);

            // TYPE TABLE
            foreach (var type in _resolver.GetResolvedTypes())
                _writer.Write<HashedType>(type.Value);

            // PROPERTY TABLE
            _writer.Write<int>(plan.PropertySpecificationGroups.Count);

            foreach (var element in plan.PropertySpecificationGroups)
            {
                // PROPERTY SPECIFICATION { Count, PropertyDefinition[] }
                _writer.Write<int>(element.Key.Definitions.Count());

                // PROPERTY SPECIFICATION TYPE (HASH CODE ONLY)
                _writer.Write<int>(element.Key.ObjectType.GetHashCode());

                foreach (var definition in element.Key.Definitions)
                {
                    // PROPERTY NAME
                    _writer.Write<string>(definition.PropertyName);

                    // PROPERTY TYPE
                    _writer.Write<HashedType>(definition.PropertyType);

                    // IS USER DEFINED
                    _writer.Write<bool>(definition.IsUserDefined);
                }

                // OBJECT REFERENCES
                _writer.Write<int>(element.Value.Count);

                foreach (var objectBase in element.Value)
                {
                    // OBJECT ID
                    _writer.Write<int>(objectBase.ObjectInfo.Id);
                }
            }

            // Recurse
            SerializeRecurse(plan.RootNode);

            // Validate
            foreach (var element in plan.UniqueReferenceDict)
            {
                if (!_serializedObjects.ContainsKey(element.Key))
                    throw new Exception("Serialization plan doesn't match the serialized manifest:  " + element.Key.Type.DeclaringType);
            }
        }

        internal IEnumerable<SerializationObjectBase> GetSerializedObjects()
        {
            return _serializedObjects.Values;
        }

        internal IList<SerializedNodeManifest> GetManifest()
        {
            return _outputManifest;
        }

        internal IEnumerable<HashedType> GetTypeTable()
        {
            return _resolver.GetResolvedTypes().Values;
        }

        private void SerializeRecurse(SerializationNodeBase node)
        {
            // Recursively identify node children and analyze by type inspection for SerializationObjectBase.
            // Select formatter for objects that are value types if no ctor / get methods are supplied

            // COLLECTION 
            if (node is SerializationCollectionNode)
            {
                var collectionNode = node as SerializationCollectionNode;

                // Serialize Collection Node (STORES CHILD COUNT)
                SerializeNodeObject(collectionNode.NodeObject);

                // Serialize Sub-Nodes (USED FOR CUSTOM COLLECTION PROPERTIES)
                foreach (var subNode in collectionNode.SubNodes)
                    SerializeRecurse(subNode);

                // Serialize Child Nodes
                foreach (var childNode in collectionNode.Children)
                    SerializeRecurse(childNode);
            }

            // NODE
            else
            {
                var objectNode = node as SerializationNode;

                // Serialize Node
                SerializeNodeObject(objectNode.NodeObject);

                // Serialize Sub-Nodes
                foreach (var subNode in objectNode.SubNodes)
                    SerializeRecurse(subNode);
            }
        }

        private void SerializeNodeObject(SerializationObjectBase nodeObject)
        {
            // Procedure
            //
            // *** For each type of serialization object store the SerializedNodeType, and the 
            //     SerializationMode ENUMS before continuing with the rest of the data.
            //
            //     Store the TYPE for the NODE OBJECT - then either the NULL, the
            //     DATA (PRIMITIVE), or Object Info ID (for all reference types). 
            //
            //     For COLLECTIONS, also store the child count, and the CollectionInterfaceType ENUM. 
            //

            // PRIMITIVE NULL
            if (nodeObject is SerializationNullPrimitive)
            {
                WriteNode(SerializedNodeType.NullPrimitive, nodeObject.Mode, nodeObject);

                return;
            }

            // NULL
            else if (nodeObject is SerializationNullObject)
            {
                WriteNode(SerializedNodeType.Null, nodeObject.Mode, nodeObject);

                return;
            }

            // PRIMITIVE
            else if (nodeObject is SerializationPrimitive)
            {
                WriteNode(SerializedNodeType.Primitive, nodeObject.Mode, nodeObject);

                return;
            }

            // *** REFERENCE TYPES

            // REFERENCE
            if (nodeObject is SerializationReference)
            {
                WriteNode(SerializedNodeType.Reference, nodeObject.Mode, nodeObject);

                return;
            }

            // STORE REFERENCE
            if (_serializedObjects.ContainsKey(nodeObject.ObjectInfo))
                throw new Exception("Duplicate reference found:  " + nodeObject.ObjectInfo.Type.GetImplementingType());

            _serializedObjects.Add(nodeObject.ObjectInfo, nodeObject);

            // VALUE (Either new sub-graph OR reference to serialized object)
            if (nodeObject is SerializationValue)
            {
                WriteNode(SerializedNodeType.Value, nodeObject.Mode, nodeObject);
            }

            // OBJECT (Either new sub-graph OR reference to serialized object)
            else if (nodeObject is SerializationObject)
            {
                WriteNode(SerializedNodeType.Object, nodeObject.Mode, nodeObject);
            }

            // COLLECTION
            else if (nodeObject is SerializationCollection)
            {
                WriteNode(SerializedNodeType.Collection, nodeObject.Mode, nodeObject);
            }
            else
                throw new Exception("Invalid SerializationObjectBase PropertySerializer.SerializeNodeObject");
        }

        private void WriteNode(SerializedNodeType type,
                               SerializationMode mode,
                               SerializationObjectBase nodeObject)
        {
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

            // var manifest = new SerializedNodeManifest();

            var objectInfo = nodeObject.ObjectInfo;
            var objectType = nodeObject.ObjectInfo.Type;

            _writer.Write<SerializedNodeType>(type);
            _writer.Write<SerializationMode>(mode);
            _writer.Write<int>(objectType.GetHashCode());

            //// Write manifest
            //manifest.Mode = mode;
            //manifest.Node = type;
            //manifest.Type = objectType.ToString();
            //manifest.ObjectId = objectInfo.Id;

            switch (type)
            {
                case SerializedNodeType.NullPrimitive:
                case SerializedNodeType.Null:
                    break;

                case SerializedNodeType.Primitive:
                    _writer.Write(objectInfo.GetObject(), objectType.GetImplementingType());
                    break;

                case SerializedNodeType.Value:
                case SerializedNodeType.Object:
                case SerializedNodeType.Reference:
                    _writer.Write<int>(objectInfo.Id);
                    break;

                case SerializedNodeType.Collection:

                    var collection = (nodeObject as SerializationCollection);

                    _writer.Write<int>(objectInfo.Id);
                    _writer.Write<CollectionInterfaceType>(collection.InterfaceType);
                    _writer.Write<int>(collection.Count);

                    // ELEMENT HASH TYPES
                    _writer.Write<int[]>(collection.ResolvedElementTypeHashCodes);

                    // Write manifest
                    //manifest.CollectionCount = (nodeObject as SerializationCollection).Count;
                    //manifest.CollectionType = (nodeObject as SerializationCollection).InterfaceType;
                    break;
                default:
                    throw new Exception("Unhandled SerializedNodeType PropertySerializer.cs");
            }

            // STORE MANIFEST OBJECT
            //_outputManifest.Add(manifest);
        }
    }
}

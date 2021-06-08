using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
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
        SimpleDictionary<int, SerializedNodeBase> _serializedObjects;

        // ORDERED NODE MANIFEST
        IList<SerializedNodeManifest> _outputManifest;

        // SINGLE INSTANCE - ONE PER RUN
        HashedTypeResolver _resolver;

        // PRIMARY OUTPUT STREAM
        ISerializationStreamWriter _writer;

        readonly RecursiveSerializerConfiguration _configuration;

        internal PropertySerializer(RecursiveSerializerConfiguration configuration)
        {
            _serializedObjects = new SimpleDictionary<int, SerializedNodeBase>();
            _outputManifest = new List<SerializedNodeManifest>();
            _resolver = new HashedTypeResolver();
            _configuration = configuration;
        }

        internal void Serialize<T>(ISerializationStreamWriter writer, T theObject)
        {
            _writer = writer;

            // SINGLE INSTANCE PER RUN - STORES ALL RESOLVED TYPES
            _resolver = new HashedTypeResolver();

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
            SerializedNodeBase.ResetCounter();

            // Run the planner
            var plan = planner.Plan(theObject);

            // Collect distinct types RESOLVER + ELEMENT TYPES (Distinct)
            var distinctTypes = _resolver.GetResolvedTypes()
                                         .Union(plan.ElementTypeDict)
                                         .DistinctBy(keyValuePair => keyValuePair.Key)
                                         .Select(keyValuePair => keyValuePair.Value)
                                         .ToList();

            // TYPE TABLE COUNT
            _writer.Write<int>(distinctTypes.Count);

            // TYPE TABLE
            foreach (var type in distinctTypes)
                _writer.Write<HashedType>(type);

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

                    // PROPERTY TYPE (HASH CODE)
                    _writer.Write<int>(definition.PropertyType.GetHashCode());

                    // IS USER DEFINED
                    _writer.Write<bool>(definition.IsUserDefined);
                }

                // OBJECT REFERENCES
                _writer.Write<int>(element.Value.Count);

                foreach (var objectBase in element.Value)
                {
                    // OBJECT ID
                    _writer.Write<int>(objectBase.Id);
                }
            }

            // Recurse
            SerializeRecurse(plan.RootNode);

            // Validate
            foreach (var objectBase in plan.ReferenceObjects)
            {
                if (!_serializedObjects.ContainsKey(objectBase.Id))
                    throw new Exception("Serialization plan doesn't match the serialized manifest:  " + objectBase.Type.DeclaringType);
            }
        }

        internal IEnumerable<SerializedNodeBase> GetSerializedObjects()
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

        private void SerializeRecurse(SerializedNodeBase node)
        {
            // Recursively identify node children and analyze by type inspection for SerializationObjectBase.
            // Select formatter for objects that are value types if no ctor / get methods are supplied

            // LEAF (PRIMITIVE)
            if (node is SerializedLeafNode)
                SerializeNodeObject(node);

            // REFERENCE
            else if (node is SerializedReferenceNode)
                SerializeNodeObject(node);

            // COLLECTION 
            else if (node is SerializedCollectionNode)
            {
                var collectionNode = node as SerializedCollectionNode;

                // Serialize Collection Node (STORES CHILD COUNT)
                SerializeNodeObject(collectionNode);

                // Serialize Sub-Nodes (USED FOR CUSTOM COLLECTION PROPERTIES)
                foreach (var subNode in collectionNode.SubNodes)
                    SerializeRecurse(subNode);

                // Serialize Child Nodes
                foreach (var childNode in collectionNode.CollectionNodes)
                    SerializeRecurse(childNode);
            }

            // OBJECT NODE
            else if (node is SerializedObjectNode)
            {
                var objectNode = node as SerializedObjectNode;

                // Serialize Node
                SerializeNodeObject(objectNode);

                // Serialize Sub-Nodes
                foreach (var subNode in objectNode.SubNodes)
                    SerializeRecurse(subNode);
            }
            else
                throw new Exception("Unhandled SerializedNodeBase type:  PropertySerializer.cs");
        }

        private void SerializeNodeObject(SerializedNodeBase nodeObject)
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

            // LEAF
            if (nodeObject is SerializedLeafNode)
            {
                switch (nodeObject.NodeType)
                {
                    case SerializedNodeType.NullPrimitive:
                    case SerializedNodeType.Null:
                    case SerializedNodeType.Primitive:
                        WriteNode(nodeObject.NodeType, nodeObject.Mode, nodeObject);
                        return;
                    default:
                        throw new Exception("Improperly typed SerializedLeafNode:  PropertySerializer.cs");
                }
            }

            // REFERENCE
            else if (nodeObject is SerializedReferenceNode)
            {
                WriteNode(nodeObject.NodeType, nodeObject.Mode, nodeObject);

                return;
            }

            // *** REFERENCE TYPES

            // STORE REFERENCE
            if (_serializedObjects.ContainsKey(nodeObject.Id))
                throw new Exception("Duplicate reference found:  " + nodeObject.Type.GetImplementingType());

            _serializedObjects.Add(nodeObject.Id, nodeObject);

            // COLLECTION : OBJECT
            if (nodeObject is SerializedCollectionNode)
            {
                if (nodeObject.NodeType != SerializedNodeType.Collection)
                    throw new Exception("Improperly typed SerializedObjectNode:  PropertySerializer.cs");

                WriteNode(nodeObject.NodeType, nodeObject.Mode, nodeObject);
            }

            // OBJECT
            else if (nodeObject is SerializedObjectNode)
            {
                if (nodeObject.NodeType != SerializedNodeType.Object)
                    throw new Exception("Improperly typed SerializedObjectNode:  PropertySerializer.cs");

                WriteNode(nodeObject.NodeType, nodeObject.Mode, nodeObject);

                return;
            }
            
            else
                throw new Exception("Invalid SerializedNodeBase PropertySerializer.SerializeNodeObject");
        }

        private void WriteNode(SerializedNodeType type,
                               SerializationMode mode,
                               SerializedNodeBase nodeObject)
        {
            // Serialize:  [ Null Primitive = 0, Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Null = 1,           Serialization Mode, Hashed Type Code ]
            // Serialize:  [ Primitive = 2,      Serialization Mode, Hashed Type Code, Primitive Value ]
            // Serialize:  [ Object = 3,         Serialization Mode, Hashed Type Code, Object Id ] (Recruse Sub - graph)
            // Serialize:  [ Reference = 4,      Serialization Mode, Hashed Type Code, Reference Object Id ]
            // Serialize:  [ Collection = 5,     Serialization Mode, Hashed Type Code, Object Id,
            //                                   Collection Interface Type,
            //                                   Child Count,
            //                                   Child Hash Type Code ] (loop) Children (Recruse Sub-graphs)

            // var manifest = new SerializedNodeManifest();

            _writer.Write<SerializedNodeType>(type);
            _writer.Write<SerializationMode>(mode);
            _writer.Write<int>(nodeObject.Type.GetHashCode());

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
                    _writer.Write(nodeObject.GetObject(), nodeObject.Type.GetImplementingType());
                    break;

                case SerializedNodeType.Object:
                    _writer.Write<int>(nodeObject.Id);
                    break;

                case SerializedNodeType.Reference:
                    _writer.Write<int>((nodeObject as SerializedReferenceNode).ReferenceId);
                    break;

                case SerializedNodeType.Collection:

                    var collection = (nodeObject as SerializedCollectionNode);

                    _writer.Write<int>(nodeObject.Id);
                    _writer.Write<CollectionInterfaceType>(collection.InterfaceType);
                    _writer.Write<int>(collection.Count);

                    // ELEMENT HASH TYPES
                    _writer.Write<int>(collection.ElementDeclaringType.GetHashCode());

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

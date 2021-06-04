using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class PropertySerializer
    {
        // Collection of formatters for serialization
        SimpleDictionary<Type, IBaseFormatter> _primitiveFormatters;

        // Used to create TYPE TABLE
        HashedTypeFormatter _hashedTypeFormatter;

        // Collection of UNIQUE objects that HAVE BEEN SERIALIZED
        SimpleDictionary<ObjectInfo, SerializationObjectBase> _serializedObjects;

        // ORDERED NODE MANIFEST
        IList<SerializedNodeManifest> _outputManifest;

        // SINGLE INSTANCE - ONE PER RUN
        ObjectInfoResolver _resolver;

        internal PropertySerializer()
        {
            _primitiveFormatters = new SimpleDictionary<Type, IBaseFormatter>();
            _serializedObjects = new SimpleDictionary<ObjectInfo, SerializationObjectBase>();
            _hashedTypeFormatter = new HashedTypeFormatter();
            _outputManifest = new List<SerializedNodeManifest>();
            _resolver = new ObjectInfoResolver();
        }

        internal void Serialize<T>(Stream stream, T theObject)
        {
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
            Write<int>(stream, _resolver.GetResolvedTypes().Count);

            // TYPE TABLE
            foreach (var type in _resolver.GetResolvedTypes())
                _hashedTypeFormatter.Write(stream, type.Key);

            // PROPERTY TABLE
            Write<int>(stream, plan.PropertySpecificationGroups.Count);

            foreach (var element in plan.PropertySpecificationGroups)
            {
                // PROPERTY SPECIFICATION { Count, PropertyDefinition[] }
                Write<int>(stream, element.Key.Definitions.Count());

                // PROPERTY SPECIFICATION TYPE (HASH CODE ONLY)
                Write<int>(stream, element.Key.ObjectType.GetHashCode());

                foreach (var definition in element.Key.Definitions)
                {
                    // PROPERTY NAME
                    Write<string>(stream, definition.PropertyName);

                    // PROPERTY TYPE
                    _hashedTypeFormatter.Write(stream, definition.PropertyType);

                    // IS USER DEFINED
                    Write<bool>(stream, definition.IsUserDefined);
                }

                // OBJECT REFERENCES
                Write<int>(stream, element.Value.Count);

                foreach (var objectBase in element.Value)
                {
                    // OBJECT ID
                    Write<int>(stream, objectBase.ObjectInfo.Id);
                }
            }

            // Recurse
            SerializeRecurse(stream, plan.RootNode);

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
            return _resolver.GetResolvedTypes().Keys;
        }

        private void SerializeRecurse(Stream stream, SerializationNodeBase node)
        {
            // Recursively identify node children and analyze by type inspection for SerializationObjectBase.
            // Select formatter for objects that are value types if no ctor / get methods are supplied

            // COLLECTION 
            if (node is SerializationCollectionNode)
            {
                var collectionNode = node as SerializationCollectionNode;

                // Serialize Collection Node (STORES CHILD COUNT)
                SerializeNodeObject(stream, collectionNode.NodeObject);

                // Serialize Sub-Nodes (USED FOR CUSTOM COLLECTION PROPERTIES)
                foreach (var subNode in collectionNode.SubNodes)
                    SerializeRecurse(stream, subNode);

                // Serialize Child Nodes
                foreach (var childNode in collectionNode.Children)
                    SerializeRecurse(stream, childNode);
            }

            // NODE
            else
            {
                var objectNode = node as SerializationNode;

                // Serialize Node
                SerializeNodeObject(stream, objectNode.NodeObject);

                // Serialize Sub-Nodes
                foreach (var subNode in objectNode.SubNodes)
                    SerializeRecurse(stream, subNode);
            }
        }

        private void SerializeNodeObject(Stream stream, SerializationObjectBase nodeObject)
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
                WriteNode(stream, SerializedNodeType.NullPrimitive, nodeObject.Mode, nodeObject);

                return;
            }

            // NULL
            else if (nodeObject is SerializationNullObject)
            {
                WriteNode(stream, SerializedNodeType.Null, nodeObject.Mode, nodeObject);

                return;
            }

            // PRIMITIVE
            else if (nodeObject is SerializationPrimitive)
            {
                WriteNode(stream, SerializedNodeType.Primitive, nodeObject.Mode, nodeObject);

                return;
            }

            // *** REFERENCE TYPES

            // REFERENCE
            if (nodeObject is SerializationReference)
            {
                WriteNode(stream, SerializedNodeType.Reference, nodeObject.Mode, nodeObject);

                return;
            }

            // STORE REFERENCE
            if (_serializedObjects.ContainsKey(nodeObject.ObjectInfo))
                throw new Exception("Duplicate reference found:  " + nodeObject.ObjectInfo.Type.GetImplementingType());

            _serializedObjects.Add(nodeObject.ObjectInfo, nodeObject);

            // VALUE (Either new sub-graph OR reference to serialized object)
            if (nodeObject is SerializationValue)
            {
                WriteNode(stream, SerializedNodeType.Value, nodeObject.Mode, nodeObject);
            }

            // OBJECT (Either new sub-graph OR reference to serialized object)
            else if (nodeObject is SerializationObject)
            {
                WriteNode(stream, SerializedNodeType.Object, nodeObject.Mode, nodeObject);
            }

            // COLLECTION
            else if (nodeObject is SerializationCollection)
            {
                WriteNode(stream, SerializedNodeType.Collection, nodeObject.Mode, nodeObject);
            }
            else
                throw new Exception("Invalid SerializationObjectBase PropertySerializer.SerializeNodeObject");
        }

        private void WriteNode(Stream stream,
                               SerializedNodeType type,
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

            var manifest = new SerializedNodeManifest();

            var objectInfo = nodeObject.ObjectInfo;
            var objectType = nodeObject.ObjectInfo.Type;

            Write(stream, type);
            Write(stream, mode);
            Write(stream, objectType.GetHashCode());

            // Write manifest
            manifest.Assembly = objectType.DeclaringAssembly;
            manifest.GenericArgumentTypes = objectType.DeclaringGenericArguments.Transform(hashedType => hashedType.ToString());
            manifest.IsGeneric = objectType.DeclaringIsGeneric;
            manifest.Mode = mode;
            manifest.Node = type;
            manifest.NodeTypeHashCode = objectType.GetHashCode();
            manifest.Type = objectType.ToString();
            manifest.ObjectId = objectInfo.Id;

            switch (type)
            {
                case SerializedNodeType.NullPrimitive:
                case SerializedNodeType.Null:
                    break;

                case SerializedNodeType.Primitive:
                    Write(stream, objectInfo.GetObject(), objectType.GetImplementingType());
                    break;

                case SerializedNodeType.Value:
                case SerializedNodeType.Object:
                case SerializedNodeType.Reference:
                    Write(stream, objectInfo.Id);
                    break;

                case SerializedNodeType.Collection:

                    var collection = (nodeObject as SerializationCollection);

                    Write(stream, objectInfo.Id);
                    Write(stream, collection.InterfaceType);
                    Write(stream, collection.Count);

                    // ELEMENT HASH TYPES
                    foreach (var elementType in collection.ResolvedElementTypes)
                        Write(stream, elementType.GetHashCode());

                    // Write manifest
                    manifest.CollectionCount = (nodeObject as SerializationCollection).Count;
                    manifest.CollectionType = (nodeObject as SerializationCollection).InterfaceType;
                    break;
                default:
                    throw new Exception("Unhandled SerializedNodeType PropertySerializer.cs");
            }

            // STORE MANIFEST OBJECT
            _outputManifest.Add(manifest);
        }

        private void Write(Stream stream, object theObject, Type theObjectType)
        {
            var formatter = SelectFormatter(theObjectType);

            formatter.Write(stream, theObject);
        }

        private void Write<T>(Stream stream, T theObject)
        {
            var formatter = SelectFormatter(typeof(T));

            formatter.Write(stream, theObject);
        }

        private IBaseFormatter SelectFormatter(Type type)
        {
            if (!_primitiveFormatters.ContainsKey(type))
                _primitiveFormatters.Add(type, CreateFormatter(type));

            return _primitiveFormatters[type];
        }

        private IBaseFormatter CreateFormatter(Type type)
        {
            if (type == typeof(bool))
                return new BooleanFormatter();

            else if (type == typeof(byte))
                return new ByteFormatter();

            else if (type == typeof(DateTime))
                return new DateTimeFormatter();

            else if (type == typeof(double))
                return new DoubleFormatter();

            else if (type == typeof(uint))
                return new UnsignedIntegerFormatter();

            else if (type == typeof(int))
                return new IntegerFormatter();

            else if (type == typeof(string))
                return new StringFormatter();

            else if (type.IsEnum)
                return new EnumFormatter(type);

            else
                throw new Exception("Unhandled type:  PropertySerializer.CreateFormatter: " + type.FullName);
        }
    }
}

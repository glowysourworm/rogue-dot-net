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
    internal class PropertySerializer
    {
        // Collection of formatters for serialization
        Dictionary<Type, IBaseFormatter> _primitiveFormatters;

        // Used to create TYPE TABLE
        HashedTypeFormatter _hashedTypeFormatter;

        // Collection of UNIQUE objects that HAVE BEEN SERIALIZED
        Dictionary<HashedObjectInfo, SerializationObjectBase> _serializedObjects;

        IList<HashedType> _typeTable;

        internal PropertySerializer()
        {
            _primitiveFormatters = new Dictionary<Type, IBaseFormatter>();
            _serializedObjects = new Dictionary<HashedObjectInfo, SerializationObjectBase>();
            _hashedTypeFormatter = new HashedTypeFormatter();
        }

        internal void Serialize<T>(Stream stream, T theObject)
        {
            var planner = new SerializationPlanner<T>();

            // Procedure
            //
            // 1) Run the planner to create reference dictionary and node tree
            // 2) STORE TYPE TABLE (KEEPS TRACK OF IMPLEMETING TYPE DISCREPANCIES)
            // 3) (Recurse) Serialize the node graph OBJECTS
            // 4) Validate OUR serialized objects against the ISerializationPlan
            //

            // Run the planner
            var plan = planner.Plan(theObject);

            // Collect discrepancies
            var typeDiscrepancies = plan.AllSerializedObjects
                                        .Where(serializedObject => serializedObject.ObjectInfo.Type.HasTypeDiscrepancy())
                                        .Select(serializedObject => serializedObject.ObjectInfo.Type)
                                        .Distinct()
                                        .ToList();

            // Store TYPE TABLE for the manifest
            _typeTable = new List<HashedType>(typeDiscrepancies);

            // Write count of types in the TYPE TABLE
            Write<int>(stream, typeDiscrepancies.Count);

            // Write TYPE TABLE
            foreach (var type in typeDiscrepancies)
                _hashedTypeFormatter.Write(stream, type);

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

        internal IEnumerable<HashedType> GetTypeTable()
        {
            return _typeTable;
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
            //     Store the TYPE for the NODE OBJECT - then either the NOTHING (NULL), the
            //     DATA (PRIMITIVE), or Hash Object Info HASH CODE (for all reference types). 
            //
            //     For COLLECTIONS, also store the child count, and the CollectionInterfaceType ENUM. 
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
            //

            // PRIMITIVE NULL
            if (nodeObject is SerializationNullPrimitive)
            {
                // Serialize:  [ NullPrimitive = 0, Serialization Mode, Hashed Type Code ]
                Write(stream, SerializedNodeType.NullPrimitive);
                Write(stream, nodeObject.Mode);
                Write(stream, nodeObject.ObjectInfo.Type.GetHashCode());

                return;
            }

            // NULL
            else if (nodeObject is SerializationNullObject)
            {
                // Serialize:  [ Null = 1, Hashed Type Code ]
                Write(stream, SerializedNodeType.Null);
                Write(stream, nodeObject.Mode);
                Write(stream, nodeObject.ObjectInfo.Type.GetHashCode());

                return;
            }

            // PRIMITIVE
            else if (nodeObject is SerializationPrimitive)
            {
                // Serialize:  [ Primitive = 2, Hashed Type Code, Value ]
                Write(stream, SerializedNodeType.Primitive);
                Write(stream, nodeObject.Mode);
                Write(stream, nodeObject.ObjectInfo.Type.GetHashCode());
                Write(stream, nodeObject.ObjectInfo.GetObject(), nodeObject.ObjectInfo.Type.GetImplementingType());

                return;
            }

            // *** REFERENCE TYPES

            // REFERENCE
            if (nodeObject is SerializationReference)
            {
                // Serialize:  [Reference = 5, Hashed Type Code, Hash Object Info Code ]
                Write(stream, SerializedNodeType.Reference);
                Write(stream, nodeObject.Mode);
                Write(stream, nodeObject.ObjectInfo.Type.GetHashCode());
                Write(stream, nodeObject.ObjectInfo.GetHashCode());

                return;
            }

            // STORE REFERENCE
            if (_serializedObjects.ContainsKey(nodeObject.ObjectInfo))
                throw new Exception("Duplicate reference found:  " + nodeObject.ObjectInfo.Type.GetImplementingType());

            _serializedObjects.Add(nodeObject.ObjectInfo, nodeObject);

            // VALUE (Either new sub-graph OR reference to serialized object)
            if (nodeObject is SerializationValue)
            {
                // (STRUCT) Serialize:  [Value = 3, Hashed Type Code, Hash Object Info Code ] (Recruse Sub - graph)
                Write(stream, SerializedNodeType.Value);
                Write(stream, nodeObject.Mode);
                Write(stream, nodeObject.ObjectInfo.Type.GetHashCode());
                Write(stream, nodeObject.ObjectInfo.GetHashCode());
            }

            // OBJECT (Either new sub-graph OR reference to serialized object)
            else if (nodeObject is SerializationObject)
            {
                // (CLASS) Serialize:  [Object = 4, Hashed Type Code, Hash Object Info Code ] (Recruse Sub - graph)
                Write(stream, SerializedNodeType.Object);
                Write(stream, nodeObject.Mode);
                Write(stream, nodeObject.ObjectInfo.Type.GetHashCode());
                Write(stream, nodeObject.ObjectInfo.GetHashCode());
            }

            // COLLECTION
            else if (nodeObject is SerializationCollection)
            {
                // Serialize:  [ Collection = 6, Hashed Type Code, Hash Object Info Code, Child Count ] (loop) Children (Recruse Sub-graphs)
                Write(stream, SerializedNodeType.Collection);
                Write(stream, nodeObject.Mode);
                Write(stream, nodeObject.ObjectInfo.Type.GetHashCode());
                Write(stream, nodeObject.ObjectInfo.GetHashCode());
                Write(stream, (nodeObject as SerializationCollection).Count);
                Write(stream, (nodeObject as SerializationCollection).InterfaceType);
            }
            else
                throw new Exception("Invalid SerializationObjectBase PropertySerializer.SerializeNodeObject");
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

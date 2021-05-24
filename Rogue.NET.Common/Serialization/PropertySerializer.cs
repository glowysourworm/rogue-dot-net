using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization
{
    public class PropertySerializer
    {
        protected enum SerializedNodeType : int
        {
            /// <summary>
            /// Serializer should store [ Null = 0, Reference HashedType ]
            /// </summary>
            Null = 0,

            /// <summary>
            /// Serializer should store [ Primitive = 1, HashedType, Value ]
            /// </summary>
            Primitive = 1,

            /// <summary>
            /// (STRUCT) Serializer should store [ Value = 2, Hash Info ] (Recruse Sub-graph)
            /// </summary>
            Value = 2,

            /// <summary>
            /// (CLASS) Serializer should store [ Object = 3, Hash Info ] (Recruse Sub-graph)
            /// </summary>
            Object = 3,

            /// <summary>
            /// Serializer should store [ Reference = 4, Hash Info ]
            /// </summary>
            Reference = 4,

            /// <summary>
            /// Serializer should store [ Collection = 5, Collection Type, Child Count ] (loop) Children (Recruse Sub-graphs)
            /// </summary>
            Collection = 5
        }

        // MSFT Default Formatter
        BinaryFormatter _defaultFormatter;

        // Collection of formatters for serialization
        Dictionary<Type, BaseFormatter> _formatters;

        // Collection of UNIQUE objects that HAVE BEEN SERIALIZED
        Dictionary<HashedObjectInfo, SerializationObjectBase> _serializedObjects;

        public PropertySerializer()
        {
            _defaultFormatter = new BinaryFormatter();
            _formatters = new Dictionary<Type, BaseFormatter>();
            _serializedObjects = new Dictionary<HashedObjectInfo, SerializationObjectBase>();
        }

        public void Serialize<T>(Stream stream, T theObject)
        {
            var planner = new SerializationPlanner<T>();

            // Procedure
            //
            // 1) Run the planner to create reference dictionary and node tree
            // 2) Create / store file header with count of ReferenceInfo objects
            // 3) Create / store TYPE HASH TABLE
            // 4) Serialize the reference dictionary ReferenceInfo OBJECT HASHES
            // 5) (Recurse) Serialize the node graph OBJECTS
            // 6) Validate OUR serialized objects against the ISerializationPlan
            //

            // Run the planner
            var plan = planner.Plan(theObject);

            // Store the file header
            var header = new PropertySerializerHeader(typeof(T), 0);

            SerializeDefault(stream, header); 

            // Serialize the reference info OBJECT HASHES
            //foreach (var element in plan.UniqueReferenceDict)
            //{
            //    SerializeDefault(stream, element.Key);
            //}

            // Recurse
            SerializeRecurse(stream, plan.RootNode);

            // Validate
            foreach(var element in plan.UniqueReferenceDict)
            {
                if (!_serializedObjects.ContainsKey(element.Key))
                    throw new Exception("Serialization plan doesn't match the serialized manifest:  " + element.Key.Type.TypeName);
            }
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
            // *** For each type of object serialized store a ENUM value FIRST (SEE ABOVE). This
            //     identifies the type of NODE stored. 
            //
            //     Then, store the TYPE for the NODE OBJECT.
            //
            // NULL (or) PRIMITIVE:  Store the NullHash OR actual primitive data to the stream
            //
            // VALUE:                (ALWAYS) Store data to the stream. Fetch data using supplied
            //                                methods.
            //
            // REFERENCE:            (SOMETIMES) Store data to the stream. If there is a reference
            //                                   in the header then just store a reference info to
            //                                   THAT reference.
            //
            // COLLECTION:           Store the child COUNT
            //

            // NULL
            if (nodeObject is SerializationNullObject)
            {
                // Serialize:  [ Null = 0, Type ]
                SerializeDefault(stream, SerializedNodeType.Null);
                SerializeHashedType(stream, nodeObject.ObjectInfo.Type);

                return;
            }

            // PRIMITIVE
            else if (nodeObject is SerializationPrimitive)
            {
                // Serialize:  [ Primitive = 1, Hashed Type, Value ]
                SerializeDefault(stream, SerializedNodeType.Primitive);
                SerializeHashedType(stream, nodeObject.ObjectInfo.Type);
                Serialize(stream, (nodeObject as SerializationPrimitive).ObjectInfo.TheObject, SelectFormatter(nodeObject.ObjectInfo.Type));

                return;
            }

            // (VALUE, OBJECT, COLLECTION):  CHECK FOR PREVIOUSLY SERIALIZED REFERENCES
            if (_serializedObjects.ContainsKey(nodeObject.ObjectInfo))
            {
                // Serialize:  [Reference = 4, Hash Info(Type, Hash Code)]
                SerializeDefault(stream, SerializedNodeType.Reference);
                SerializeHashedType(stream, nodeObject.ObjectInfo.Type);
                SerializeDefault(stream, nodeObject.ObjectInfo.HashCode);

                return;
            }
            // STORE REFERENCE
            else
                _serializedObjects.Add(nodeObject.ObjectInfo, nodeObject);


            // VALUE (Either new sub-graph OR reference to serialized object)
            if (nodeObject is SerializationValue)
            {
                // (STRUCT) Serialize:  [Value = 2, Hash Info] (Recruse Sub - graph)
                SerializeDefault(stream, SerializedNodeType.Value);
                SerializeHashedType(stream, nodeObject.ObjectInfo.Type);
            }

            // OBJECT (Either new sub-graph OR reference to serialized object)
            else if (nodeObject is SerializationObject)
            {
                // (CLASS) Serialize:  [Value = 2, Hash Info] (Recruse Sub - graph)
                SerializeDefault(stream, SerializedNodeType.Object);
                SerializeHashedType(stream, nodeObject.ObjectInfo.Type);
            }

            // COLLECTION
            else if (nodeObject is SerializationCollection)
            {
                // Serialize:  [ Collection = 5, Collection Type, Child Count ] (loop) Children (Recruse Sub-graphs)
                SerializeDefault(stream, SerializedNodeType.Collection);
                SerializeHashedType(stream, nodeObject.ObjectInfo.Type);
                SerializeDefault(stream, (nodeObject as SerializationCollection).Count);
            }
            else
                throw new Exception("Invalid SerializationObjectBase PropertySerializer.SerializeNodeObject");
        }

        // Serialize using default formatter (MSFT)
        private void SerializeDefault(Stream stream, object theObject)
        {
            _defaultFormatter.Serialize(stream, theObject);
        }

        private void SerializeHashedType(Stream stream, HashedType hashedType)
        {
            // USING DEFAULT FORMATTER FOR NOW
            SerializeDefault(stream, hashedType.TypeName);
            SerializeDefault(stream, hashedType.AssemblyName);
        }

        // Serialize using specified formatter
        private void Serialize(Stream stream, object theObject, BaseFormatter formatter)
        {
            formatter.Write(stream, theObject);
        }

        private BaseFormatter SelectFormatter(HashedType hashedType)
        {
            return SelectFormatter(hashedType.Resolve());
        }

        private BaseFormatter SelectFormatter(Type type)
        {
            if (!_formatters.ContainsKey(type))
                _formatters.Add(type, CreateFormatter(type));

            return _formatters[type];
        }

        private BaseFormatter CreateFormatter(Type type)
        {
            // DEFAULT FOR PRIMITIVE TYPES
            if (PrimitiveFormatter.IsPrimitive(type))
                return new PrimitiveFormatter(type);

            else
                throw new Exception("Unhandled type:  PropertySerializer.CreateFormatter: " + type.FullName);
        }
    }
}

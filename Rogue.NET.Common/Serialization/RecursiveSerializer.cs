using System.IO;

namespace Rogue.NET.Common.Serialization
{
    internal enum SerializedNodeType : byte
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

    /// <summary>
    /// Serializer that performs depth-first serialization / deserialization
    /// </summary>
    public class RecursiveSerializer<T>
    {
        PropertySerializer _serializer;
        PropertyDeserializer _deserializer;

        public RecursiveSerializer()
        {
            _serializer = new PropertySerializer();
            _deserializer = new PropertyDeserializer();
        }

        public void Serialize(Stream stream, T theObject)
        {
            _serializer.Serialize(stream, theObject);
        }

        public T Deserialize(Stream stream)
        {
            return _deserializer.Deserialize<T>(stream);
        }
    }
}

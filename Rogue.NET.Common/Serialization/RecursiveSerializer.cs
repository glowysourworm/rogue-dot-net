using KellermanSoftware.CompareNetObjects;

using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Manifest;

using System;
using System.Collections.Generic;
using System.IO;

namespace Rogue.NET.Common.Serialization
{
    public enum SerializedNodeType : byte
    {
        /// <summary>
        /// Serializer should store [ NullPrimitive = 0, Reference HashedType ]
        /// </summary>
        NullPrimitive = 0,

        /// <summary>
        /// Serializer should store [ Null = 0, Reference HashedType ]
        /// </summary>
        Null = 1,

        /// <summary>
        /// Serializer should store [ Primitive = 1, HashedType, Value ]
        /// </summary>
        Primitive = 2,

        /// <summary>
        /// (STRUCT) Serializer should store [ Value = 2, Hash Info ] (Recruse Sub-graph)
        /// </summary>
        Value = 3,

        /// <summary>
        /// (CLASS) Serializer should store [ Object = 3, Hash Info ] (Recruse Sub-graph)
        /// </summary>
        Object = 4,

        /// <summary>
        /// Serializer should store [ Reference = 4, Hash Info ]
        /// </summary>
        Reference = 5,

        /// <summary>
        /// Serializer should store [ Collection = 5, Collection Type, Child Count ] (loop) Children (Recruse Sub-graphs)
        /// </summary>
        Collection = 6
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

        public RecursiveSerializer(Type objectType)
        {
            _serializer = new PropertySerializer();
            _deserializer = new PropertyDeserializer();
        }

        /// <summary>
        /// Clears out context and sets up for a new run
        /// </summary>
        public void Clear()
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

        public SerializationManifest CreateManifest()
        {
            return new SerializationManifest(_serializer.GetTypeTable(), 
                                             _serializer.GetManifest(), 
                                             _deserializer.GetTypeTable(), 
                                             _deserializer.GetManifest());
        }

        public List<SerializedNodeDifference> CreateDifferenceList()
        {
            var serializerOutput = _serializer.GetManifest();
            var deserializerOutput = _deserializer.GetManifest();

            var differences = new List<SerializedNodeDifference>();
            var compareLogic = new CompareLogic(new ComparisonConfig());

            for (int index = 0; index < Math.Max(serializerOutput.Count, deserializerOutput.Count); index++)
            {
                if (index >= serializerOutput.Count)
                    differences.Add(new SerializedNodeDifference()
                    {
                        SerializedNode = null,
                        DeserializedNode = deserializerOutput[index]
                    });

                else if (index >= deserializerOutput.Count)
                    differences.Add(new SerializedNodeDifference()
                    {
                        SerializedNode = serializerOutput[index],
                        DeserializedNode = null
                    });

                else if (!compareLogic.Compare(serializerOutput[index], deserializerOutput[index]).AreEqual)
                    differences.Add(new SerializedNodeDifference()
                    {
                        SerializedNode = serializerOutput[index], 
                        DeserializedNode = deserializerOutput[index]
                    });
            }

            return differences;
        }
    }
}

using KellermanSoftware.CompareNetObjects;

using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.IO;
using Rogue.NET.Common.Serialization.Manifest;

using System;
using System.Collections.Generic;
using System.IO;

namespace Rogue.NET.Common.Serialization
{
    /// <summary>
    /// Serializer that performs depth-first serialization / deserialization
    /// </summary>
    public class RecursiveSerializer<T>
    {
        PropertySerializer _serializer;
        PropertyDeserializer _deserializer;

        public RecursiveSerializer(RecursiveSerializerConfiguration configuration)
        {
            _serializer = new PropertySerializer(configuration);
            _deserializer = new PropertyDeserializer(configuration);
        }

        public RecursiveSerializer(Type objectType, RecursiveSerializerConfiguration configuration)
        {
            _serializer = new PropertySerializer(configuration);
            _deserializer = new PropertyDeserializer(configuration);
        }

        public void Serialize(Stream stream, T theObject)
        {
            var serializationStream = new SerializationStream(stream);

            _serializer.Serialize(serializationStream, theObject);
        }

        public T Deserialize(Stream stream)
        {
            var serializationStream = new SerializationStream(stream);

            return _deserializer.Deserialize<T>(serializationStream);
        }

        public SerializationManifest CreateManifest()
        {
            return new SerializationManifest(_serializer.GetManifest(), 
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
                        SerializedNode = SerializedNodeManifest.Empty,
                        DeserializedNode = deserializerOutput[index]
                    });

                else if (index >= deserializerOutput.Count)
                    differences.Add(new SerializedNodeDifference()
                    {
                        SerializedNode = serializerOutput[index],
                        DeserializedNode = SerializedNodeManifest.Empty
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

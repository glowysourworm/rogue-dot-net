using Rogue.NET.Common.Serialization;
using Rogue.NET.Common.Utility;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rogue.NET.Common.Utility
{
    /// <summary>
    /// Use for cloning object graphs. Binding straight to the model graph is causing
    /// issues during serialization. Use cloning to detatch event hooks.
    /// </summary>
    public static class BinarySerializer
    {
        public enum SerializationMode
        {
            MSFT = 0,
            RecursiveSerializer = 1
        }

        public static T BinaryCopy<T>(T theObject, SerializationMode mode)
        {
            switch (mode)
            {
                case SerializationMode.MSFT:
                    return BinaryCopyMSFT(theObject);
                case SerializationMode.RecursiveSerializer:
                    return BinaryCopy(theObject);
                default:
                    throw new System.Exception("Unhandled SerializationMode type:  BinarySerializer.cs");
            }
        }

        public static byte[] Serialize<T>(T theObject, SerializationMode mode)
        {
            switch (mode)
            {
                case SerializationMode.MSFT:
                    return SerializeMSFT(theObject);
                case SerializationMode.RecursiveSerializer:
                    return Serialize(theObject);
                default:
                    throw new System.Exception("Unhandled SerializationMode type:  BinarySerializer.cs");
            }
        }

        public static T Deserialize<T>(byte[] buffer, SerializationMode mode)
        {
            switch (mode)
            {
                case SerializationMode.MSFT:
                    return DeserializeMSFT<T>(buffer);
                case SerializationMode.RecursiveSerializer:
                    return Deserialize<T>(buffer);
                default:
                    throw new System.Exception("Unhandled SerializationMode type:  BinarySerializer.cs");
            }
        }

        public static void SerializeToFile<T>(string file, T theObject, SerializationMode mode)
        {
            switch (mode)
            {
                case SerializationMode.MSFT:
                    SerializeToFileMSFT(file, theObject);
                    break;
                case SerializationMode.RecursiveSerializer:
                    SerializeToFile(file, theObject);
                    break;
                default:
                    throw new System.Exception("Unhandled SerializationMode type:  BinarySerializer.cs");
            }
        }

        public static T DeserializeFromFile<T>(string file, SerializationMode mode)
        {
            switch (mode)
            {
                case SerializationMode.MSFT:
                    return DeserializeFromFileMSFT<T>(file);
                case SerializationMode.RecursiveSerializer:
                    return DeserializeFromFile<T>(file);
                default:
                    throw new System.Exception("Unhandled SerializationMode type:  BinarySerializer.cs");
            }
        }

        #region (private) SerializationMode = MSFT
        private static T BinaryCopyMSFT<T>(T theObject)
        {
            var buffer = SerializeMSFT(theObject);

            return DeserializeMSFT<T>(buffer);
        }

        private static byte[] SerializeMSFT<T>(T theObject)
        {
            var serializer = new BinaryFormatter();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, theObject);

                return stream.GetBuffer();
            }
        }
        private static T DeserializeMSFT<T>(byte[] buffer)
        {
            var serializer = new BinaryFormatter();

            using (var stream = new MemoryStream(buffer))
            {
                return (T)serializer.Deserialize(stream);
            }
        }

        private static void SerializeToFileMSFT<T>(string file, T theObject)
        {
            File.WriteAllBytes(file, SerializeMSFT(theObject));
        }

        private static T DeserializeFromFileMSFT<T>(string file)
        {
            var bytes = File.ReadAllBytes(file);

            return DeserializeMSFT<T>(bytes);
        }
        #endregion

        #region (private) SerializationMode = RecursiveSerializer
        private static T BinaryCopy<T>(T theObject)
        {
            var buffer = Serialize(theObject);

            return Deserialize<T>(buffer);
        }

        private static byte[] Serialize<T>(T theObject)
        {
            var serializer = new RecursiveSerializer<T>();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, theObject);

                return stream.GetBuffer();
            }
        }

        private static T Deserialize<T>(byte[] buffer)
        {
            var serializer = new RecursiveSerializer<T>();

            using (var stream = new MemoryStream(buffer))
            {
                return serializer.Deserialize(stream);
            }
        }

        private static void SerializeToFile<T>(string file, T theObject)
        {
            File.WriteAllBytes(file, Serialize(theObject));
        }

        private static T DeserializeFromFile<T>(string file)
        {
            var bytes = File.ReadAllBytes(file);

            return Deserialize<T>(bytes);
        }
        #endregion
    }
}

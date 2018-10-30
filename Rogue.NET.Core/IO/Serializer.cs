using Rogue.NET.Common.Utility;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rogue.NET.Core.IO
{
    /// <summary>
    /// Use for cloning object graphs. Binding straight to the model graph is causing
    /// issues during serialization. Use cloning to detatch event hooks.
    /// </summary>
    public static class BinarySerializer
    {
        public static byte[] Serialize(object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }

        public static object Deserialize(byte[] buffer)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(buffer))
            {
                return formatter.Deserialize(stream);
            }
        }

        public static byte[] SerializeAndCompress(object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                var buffer = stream.GetBuffer();
                return ZipEncoder.Compress(buffer);
            }
        }

        public static T DeserializeAndDecompress<T>(byte[] compressedBuffer)
        {
            var buffer = ZipEncoder.Decompress(compressedBuffer);
            return (T)Deserialize(buffer);
        }

        public static object Copy(object obj)
        {
            var buffer = Serialize(obj);
            return Deserialize(buffer);
        }
    }
}

using Rogue.NET.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model
{
    public interface ISerializer
    {
        byte[] Serialize(object obj);
        object Deserialize(byte[] buffer);

        byte[] SerializeAndCompress(object obj);
        object DeserializeAndDecompress(byte[] compressedBuffer);

        object Copy(object obj);
    }

    /// <summary>
    /// Use for cloning object graphs. Binding straight to the model graph is causing
    /// issues during serialization. Use cloning to detatch event hooks.
    /// </summary>
    public class BinarySerializer : ISerializer
    {
        public byte[] Serialize(object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }

        public object Deserialize(byte[] buffer)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(buffer))
            {
                return formatter.Deserialize(stream);
            }
        }

        public byte[] SerializeAndCompress(object obj)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                var buffer = stream.GetBuffer();
                return ZipEncoder.Compress(buffer);
            }
        }

        public object DeserializeAndDecompress(byte[] compressedBuffer)
        {
            var buffer = ZipEncoder.Decompress(compressedBuffer);
            return Deserialize(buffer);
        }

        public object Copy(object obj)
        {
            var buffer = Serialize(obj);
            return Deserialize(buffer);
        }
    }
}

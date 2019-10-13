using Rogue.NET.Common.Utility;
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

        public static void SerializeToFile(string file, object obj)
        {
            File.WriteAllBytes(file, Serialize(obj));
        }

        public static object DeserializeFromFile(string file)
        {
            var bytes = File.ReadAllBytes(file);
            return Deserialize(bytes);
        }
    }
}

using Rogue.NET.Common.Utility;

using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.IO
{
    [Serializable]
    public class Compressed<K, V> : ISerializable
    {
        byte[] _buffer;

        public K Key { get; private set; }

        public Compressed(K key, V theObject)
        {
            this.Key = key;

            Checkin(theObject);
        }

        public Compressed(SerializationInfo info, StreamingContext context)
        {
            _buffer = (byte[])info.GetValue("Buffer", typeof(byte[]));
            this.Key = (K)info.GetValue("Key", typeof(K));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Buffer", _buffer);
            info.AddValue("Key", this.Key);
        }

        public void Checkin(V newObject)
        {
            var buffer = BinarySerializer.Serialize(newObject);
            var compressedBuffer = ZipEncoder.Compress(buffer);

            _buffer = compressedBuffer;
        }

        public V Checkout()
        {
            var decompressedBuffer = ZipEncoder.Decompress(_buffer);
            var theObject = BinarySerializer.Deserialize(decompressedBuffer);

            return (V)theObject;
        }
    }
}

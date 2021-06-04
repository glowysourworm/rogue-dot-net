using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Common.Collection
{
    /// <summary>
    /// Serializable dictionary
    /// </summary>
    [Serializable]
    public class SimpleDictionary<K, V> : Dictionary<K, V>, IRecursiveSerializable, ISerializable
    {
        public SimpleDictionary() : base()
        { }

        public SimpleDictionary(int capacity) : base(capacity)
        { }

        public SimpleDictionary(IDictionary<K, V> dictionary) : base(dictionary)
        { }

        public SimpleDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var count = (int)info.GetValue("Count", typeof(int));

            for (int index = 0; index < count; index++)
            {
                var key = (K)info.GetValue("Key" + index.ToString(), typeof(K));
                var value = (V)info.GetValue("Value" + index.ToString(), typeof(V));

                Add(key, value);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var counter = 0;

            info.AddValue("Count", this.Count);

            foreach (var element in this)
            {
                info.AddValue("Key" + counter.ToString(), element.Key);
                info.AddValue("Value" + counter.ToString(), element.Value);

                counter++;
            }
        }

        public SimpleDictionary(IPropertyReader reader) : base()
        {
            var count = reader.Read<int>("Count");

            for (int index = 0; index < count; index++)
            {
                var key = reader.Read<K>("Key" + index.ToString());
                var value = reader.Read<V>("Value" + index.ToString());

                Add(key, value);
            }
        }

        public void GetProperties(IPropertyWriter writer)
        {
            var counter = 0;

            writer.Write("Count", this.Count);

            foreach (var element in this)
            {
                writer.Write("Key" + counter.ToString(), element.Key);
                writer.Write("Value" + counter.ToString(), element.Value);

                counter++;
            }
        }
    }
}

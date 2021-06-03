using Rogue.NET.Common.Serialization.Interface;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Collection
{
    /// <summary>
    /// Serializable dictionary
    /// </summary>
    [Serializable]
    public class SimpleDictionary<K, V> : Dictionary<K, V>, IRecursiveSerializable
    {
        public SimpleDictionary() : base()
        { }

        public SimpleDictionary(IDictionary<K, V> dictionary) : base(dictionary)
        { }

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

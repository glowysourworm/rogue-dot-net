using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Common.Collection
{
    [Serializable]
    public class SimpleList<T> : List<T>, ISerializable
    {
        public SimpleList()
        { }

        public SimpleList(int capacity) : base(capacity)
        { }

        public SimpleList(IEnumerable<T> collection) : base(collection)
        { }

        public SimpleList(SerializationInfo info, StreamingContext context)
        {
            var count = info.GetInt32("Count");

            for (int i = 0; i < count; i++)
            {
                var item = (T)info.GetValue("Item" + i.ToString(), typeof(T));

                Add(item);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Count", this.Count);

            for (int i = 0; i < this.Count; i++)
                info.AddValue("Item" + i.ToString(), this[i]);
        }
    }
}

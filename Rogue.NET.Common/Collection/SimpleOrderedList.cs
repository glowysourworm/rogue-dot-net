using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rogue.NET.Common.Collection
{
    /// <summary>
    /// A simple ordered list implementation - sorts items when inserted and removed
    /// </summary>
    [Serializable]
    public class SimpleOrderedList<T> : IList<T>, ISerializable
    {
        List<T> _list;

        public SimpleOrderedList()
        {
            _list = new List<T>();
        }

        public SimpleOrderedList(SerializationInfo info, StreamingContext context)
        {
            var count = info.GetInt32("Count");

            _list = new List<T>(count);

            for (int i = 0; i < _list.Count; i++)
            {
                _list.Add((T)info.GetValue("Item" + i.ToString(), typeof(T)));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Count", _list.Count);

            for (int i = 0; i < _list.Count; i++)
            {
                info.AddValue("Item" + i.ToString(), _list[i]);
            }
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                _list[index] = value;

                // TODO: IMPLEMENT BINARY SEARCH INSERT
                _list.Sort();
            }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            _list.Add(item);

            // TODO: IMPLEMENT BINARY SEARCH INSERT
            _list.Sort();
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);

            // TODO: IMPLEMENT BINARY SEARCH INSERT
            _list.Sort();
        }

        public bool Remove(T item)
        {
            var result = _list.Remove(item);

            if (!result)
                return false;

            // TODO: IMPLEMENT BINARY SEARCH INSERT
            _list.Sort();

            return result;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Runtime.Serialization;

namespace Rogue.NET.Common.Collections
{
    [Serializable]
    public class SerializableDictionary<K, V> : IDictionary<K,V>, ISerializable
    {
        public event EventHandler CollectionAltered;

        List<K> _keys = new List<K>();
        List<V> _vals = new List<V>();

        public SerializableDictionary()
        {
        }
        public SerializableDictionary(SerializationInfo info, StreamingContext context)
        {
            _keys = (List<K>)info.GetValue("Keys", typeof(List<K>));
            _vals = (List<V>)info.GetValue("Values", typeof(List<V>));
        }

        public void Add(K key, V value)
        {
            if (_keys.Any(z => z.Equals(key)))
                throw new Exception("Dictionary already contains key - " + key.ToString());

            _keys.Add(key);
            _vals.Add(value);

            if (CollectionAltered != null)
                CollectionAltered(this, new System.EventArgs());
        }
        public bool ContainsKey(K key)
        {
            return _keys.Any(z => z.Equals(key));
        }
        public ICollection<K> Keys
        {
            get { return _keys; }
        }

        public bool Remove(K key)
        {
            int idx = _keys.IndexOf(key);
            _keys.RemoveAt(idx);
            _vals.RemoveAt(idx);

            if (CollectionAltered != null)
                CollectionAltered(this, new System.EventArgs());

            return true;
        }

        public bool TryGetValue(K key, out V value)
        {
            int idx = _keys.IndexOf(key);
            value = _vals[idx];
            return true;
        }

        public ICollection<V> Values
        {
            get { return _vals; }
        }

        public V this[K key]
        {
            get
            {
                int idx = _keys.IndexOf(key);
                return _vals[idx];
            }
            set
            {
                int idx = _keys.IndexOf(key);
                _vals[idx] = value;
            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
            if (CollectionAltered != null)
                CollectionAltered(this, new System.EventArgs());
        }

        public void Clear()
        {
            _keys.Clear();
            _vals.Clear();
            if (CollectionAltered != null)
                CollectionAltered(this, new System.EventArgs());
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            int idx = _keys.IndexOf(item.Key);
            return _vals[idx].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>();
            for (int i = 0; i < _keys.Count; i++)
            {
                KeyValuePair<K, V> kvp = new KeyValuePair<K, V>(_keys[i], _vals[i]);
                list.Add(kvp);
            }
            return list.GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Keys", _keys);
            info.AddValue("Values", _vals);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keys.GetEnumerator();
        }
    }
}

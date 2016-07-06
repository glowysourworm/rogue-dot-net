using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Rogue.NET.Common.Collections
{

    public enum CollectionAction
    {
        Add,
        Remove,
        PropertyChanged
    }
    public class CollectionAlteredEventArgs : EventArgs
    {
        public object Item { get; set; }
        public CollectionAction Action { get; set; }
    }

    [Serializable]
    public class SerializableObservableCollection<T> : ObservableCollection<T>, ISerializable
    {
        /// <summary>
        /// Collection changed workaround - using NotifyCollectionChangedAction.Add was adding items to the 
        /// different views
        /// </summary>
        public event EventHandler<CollectionAlteredEventArgs> CollectionAltered;

        public SerializableObservableCollection() { }
        public SerializableObservableCollection(SerializationInfo info, StreamingContext context)
        {
            var count = info.GetInt32("Length");
            for (int i = 0; i < count; i++)
                this.Add((T)info.GetValue("Item" + i.ToString(), typeof(T)));
        }
        public SerializableObservableCollection(IEnumerable<T> collection)
        {
            foreach (T t in collection)
                this.Add(t);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T t in collection)
                this.Add(t);
        }
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if (item is INotifyPropertyChanged)
            {
                INotifyPropertyChanged notify = item as INotifyPropertyChanged;
                notify.PropertyChanged += new PropertyChangedEventHandler(OnItemPropertyChanged);
            }
            if (CollectionAltered != null)
                CollectionAltered(this, new CollectionAlteredEventArgs() { Action = CollectionAction.Add, Item = item });
        }
        protected override void RemoveItem(int index)
        {
            T item = this[index];
            if (item is INotifyPropertyChanged)
            {
                INotifyPropertyChanged notify = item as INotifyPropertyChanged;
                notify.PropertyChanged -= new PropertyChangedEventHandler(OnItemPropertyChanged);
            }
            base.RemoveItem(index);
            if (CollectionAltered != null)
            {
                CollectionAltered(this, new CollectionAlteredEventArgs() { Action = CollectionAction.Remove, Item = item });
            }
        }

        protected override void ClearItems()
        {
            //base.ClearItems();
            if (this.Count > 0)
                base.ClearItems();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CollectionAltered != null)
                CollectionAltered(this, new CollectionAlteredEventArgs() { Action = CollectionAction.PropertyChanged, Item = sender });
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Length", this.Count);
            for (int i = 0; i < this.Count; i++)
                info.AddValue("Item" + i.ToString(), this[i]);
        }
    }

    public static class IEnumerableExtension
    {
        public static SerializableObservableCollection<T> ToCollection<T>(this IEnumerable<T> collection)
        {
            if (collection is SerializableObservableCollection<T>)
                return (SerializableObservableCollection<T>)collection;

            return new SerializableObservableCollection<T>(collection);
        }
    }
}
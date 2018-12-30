using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Rogue.NET.Common.ViewModel
{
    /// <summary>
    /// Supports paging for UI bound collections.
    /// 
    /// NOTE*** For enumerating ALL items
    /// MUST use special extension methods PagedForEach, PagedFirstOrDefault, PagedFirst, PagedAny, PagedCount, PagedWhere.
    /// </summary>
    public class PagedObservableCollection<T> 
        : IEnumerable<T>, IList<T>, INotifyPropertyChanged, INotifyCollectionChanged where T : class
    {
        readonly List<List<T>> _pages;
        readonly int _pageSize;

        int _pageIndex = 0;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public PagedObservableCollection(int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentException("Must use page size of 1 or greater for paged collection");

            _pageSize = pageSize;
            _pages = new List<List<T>>();

            // Initialize with a single empty page
            _pages.Add(new List<T>());
        }

        /// <summary>
        /// Returns current page number
        /// </summary>
        public int PageNumber
        {
            get { return _pageIndex + 1; }
        }

        /// <summary>
        /// Returns total number of pages
        /// </summary>
        public int PageCount
        {
            get { return _pages.Count; }
        }

        public void NextPage()
        {
            // Reset past the final page
            if (_pageIndex == _pages.Count - 1)
                _pageIndex = 0;

            else
                _pageIndex++;

            OnPropertyChanged("PageNumber");
            OnPropertyChanged("Count");

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void PreviousPage()
        {
            // Cycle around to final page
            if (_pageIndex == 0)
                _pageIndex = _pages.Count - 1;

            else
                _pageIndex--;

            OnPropertyChanged("PageNumber");
            OnPropertyChanged("Count");

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Returns number of items in the current page
        /// </summary>
        public int Count
        {
            get { return _pages[_pageIndex].Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { throw new NotSupportedException("Indexing not supported for "); }
            set { throw new NotSupportedException("Indexing not supported for "); }
        }

        public IEnumerable<T> GetCurrentPage()
        {
            return _pages[_pageIndex];
        }

        public void ResetPageNumber()
        {
            _pageIndex = 0;

            OnPropertyChanged("PageNumber");
            OnPropertyChanged("Count");

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public int IndexOf(T item)
        {
            throw new NotSupportedException("IndexOf not supported for paged collection. Please use Contains");
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Inserting not supported for paged collection. Please use Add");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Remove not supported for paged collection.");
        }

        public void Add(T item)
        {
            // 1) Check to see that current page is set to last page
            // 2) Check page capacity reached
            // 3) Add new item
            // 4) Notify Listeners

            // Just set the page index to the last page
            _pageIndex = _pages.Count - 1;

            // Create a new page if capacity is reached
            if (_pages[_pageIndex].Count == _pageSize)
            {
                _pages.Add(new List<T>());
                _pageIndex++;
            }

            // Insert new item
            _pages[_pageIndex].Add(item);

            // Notify Listeners
            OnPropertyChanged("PageNumber");
            OnPropertyChanged("PageCount");
            OnPropertyChanged("Count");

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Add(item);
        }

        public void Clear()
        {
            _pages.Clear();
            _pages.Add(new List<T>());
            _pageIndex = 0;

            // Notify Listeners
            OnPropertyChanged("PageNumber");
            OnPropertyChanged("PageCount");
            OnPropertyChanged("Count");

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _pages.Any(x => x.Any(y => y.Equals(item)));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException("CopyTo not supported for paged collection.");
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException("Remove not supported for paged collection.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _pages[_pageIndex].GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pages[_pageIndex].GetEnumerator();
        }

        /// <summary>
        /// Enumerates all pages to perform the action
        /// </summary>
        public void PagedForEach(Action<T> action)
        {
            foreach (var page in _pages)
            {
                foreach (var item in page)
                    action(item);
            }
        }

        /// <summary>
        /// Enumerates all pages to perform the search
        /// </summary>
        public T PagedFirstOrDefault(Func<T, bool> comparer)
        {
            var result = default(T);

            foreach(var page in _pages)
            {
                result = page.FirstOrDefault(x => comparer(x));

                if (result != default(T))
                    return result;
            }

            return default(T);
        }

        /// <summary>
        /// Enumerates all pages to perform the search
        /// </summary>
        public T PagedFirst(Func<T, bool> comparer)
        {
            return _pages.Select(x => x.Find(z => comparer(z)))
                         .Where(x => x != default(T))
                         .First(x => comparer(x));
        }

        /// <summary>
        /// Enumerates all pages to perform the query
        /// </summary>
        public bool PagedAny(Func<T, bool> condition)
        {
            return _pages.Any(x => x.Any(z => condition(z)));
        }

        /// <summary>
        /// Enumerates all pages to perform the count
        /// </summary>
        public int PagedCount(Func<T, bool> condition)
        {
            return _pages.Sum(x => x.Count(z => condition(z)));
        }

        /// <summary>
        /// Enumerates all pages to perform the filter; and returns a single collection with the result set
        /// </summary>
        public IEnumerable<T> PagedWhere(Func<T, bool> condition)
        {
            return _pages.SelectMany(x => x.Where(z => condition(z)));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

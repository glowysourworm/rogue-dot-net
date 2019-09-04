using System;
using System.Collections.Generic;
using System.Linq;

using MoreLinq;

namespace Rogue.NET.Common.Extension
{
    /// <summary>
    /// Enumerable extension that uses some home-grown methods with other libraries (to hide the
    /// external namespaces) to provide more linq extensions
    /// </summary>
    public static class EnumerableExtension
    {
        public static IEnumerable<TResult> FullJoin<T, TSecond, TKey, TResult>(
            this IEnumerable<T> collection,
            IEnumerable<TSecond> secondCollection,
            Func<T, TKey> firstKeySelector,
            Func<TSecond, TKey> secondKeySelector,
            Func<T, TResult> firstSelector,
            Func<TSecond, TResult> secondSelector,
            Func<T, TSecond, TResult> bothSelector)
        {
            return MoreEnumerable.FullJoin(collection, secondCollection, 
                                           firstKeySelector, 
                                           secondKeySelector, 
                                           firstSelector, 
                                           secondSelector, 
                                           bothSelector);
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var element in collection)
                action(element);
        }

        /// <summary>
        /// Returns the item of a collection corresponding to the max of some selected value
        /// </summary>
        public static T MaxWith<T, TValue>(this IEnumerable<T> collection, Func<T, TValue> valueSelector) where TValue : IComparable
        {
            T max = default(T);
            TValue maxValue = default(TValue);

            // Iterate through collection to get the greatest item for the specified selected value
            foreach (var item in collection)
            {
                var nextValue = valueSelector(item);

                if (maxValue.Equals(default(TValue)))
                {
                    maxValue = nextValue;
                    max = item;
                }

                else if (nextValue.CompareTo(maxValue) > 0)
                {
                    maxValue = nextValue;
                    max = item;
                }
            }

            return max;
        }

        /// <summary>
        /// Returns the item of a collection corresponding to the mix of some selected value
        /// </summary>
        public static T MinWhere<T, TValue>(this IEnumerable<T> collection, Func<T, TValue> valueSelector) where TValue : IComparable
        {
            T min = default(T);
            TValue minValue = default(TValue);

            // Iterate through collection to get the greatest item for the specified selected value
            foreach (var item in collection)
            {
                var nextValue = valueSelector(item);

                if (minValue.Equals(default(TValue)))
                {
                    minValue = nextValue;
                    min = item;
                }

                else if (nextValue.CompareTo(minValue) < 0)
                {
                    minValue = nextValue;
                    min = item;
                }
            }

            return min;
        }

        /// <summary>
        /// Repeats a transform on an object to create a collection of transformed items. (Somewhat like a "Clone Many")
        /// </summary>
        /// <typeparam name="T">input type</typeparam>
        /// <typeparam name="TTransform">transform type</typeparam>
        /// <param name="item">the source item</param>
        /// <param name="transform">the transform method</param>
        /// <param name="repeatCount">the number of times to repeat the transform</param>
        /// <returns>New collection of type TTransform</returns>
        public static ICollection<TTransform> TransformMany<T, TTransform>(this T item, Func<T, TTransform> transform, int repeatCount)
        {
            var result = new List<TTransform>();
            for (int i = 0; i < repeatCount; i++)
                result.Add(transform(item));

            return result;
        }

        /// <summary>
        /// Filters out some elements of a list and return those elements
        /// </summary>
        /// <param name="filter">filter to match elements by</param>
        public static IEnumerable<T> Filter<T>(this IList<T> list, Func<T, bool> filter)
        {
            var result = new List<T>();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (filter(list[i]))
                {
                    // Store with results
                    result.Add(list[i]);

                    // Remove from the list
                    list.RemoveAt(i);
                }
            }

            return result;
        }

        /// <summary>
        /// Forces actualization of Lazy enumerables
        /// </summary>
        public static IEnumerable<T> Actualize<T>(this IEnumerable<T> collection)
        {
            return collection.ToList();
        }

        /// <summary>
        /// Selects a random element from the sequence
        /// </summary>
        public static T PickRandom<T>(this IEnumerable<T> collection)
        {
            return collection.Any() ? collection.RandomSubset(1).First() : default(T);
        }

        /// <summary>
        /// Copies element references into a new collection
        /// </summary>
        public static IEnumerable<T> Copy<T>(this IEnumerable<T> collection)
        {
            var result = new List<T>(collection.Count());

            foreach (var item in collection)
                result.Add(item);

            return result;
        }

        /// <summary>
        /// Returns elements that have a non-unique property
        /// </summary>
        public static IEnumerable<T> NonUnique<T, K>(this IEnumerable<T> collection, Func<T, K> selector) 
        {
            var counts = collection.Select(x => new
            {
                Item = x,
                Count = collection.Where(z => selector(z).Equals(selector(x))).Count()
            });

            return counts.Where(x => x.Count > 1).Select(x => x.Item);
        }

        /// <summary>
        /// Returns all elements in the collection that match the given item using the provided key selector
        /// </summary>
        public static IEnumerable<T> Gather<T, K>(this IEnumerable<T> collection, T item, Func<T, K> keySelector)
        {
            return collection.Where(x => keySelector(x).Equals(keySelector(item)));
        }

        /// <summary>
        /// Returns elements that are distinct up to the selected key (property). This would be the first
        /// such element in a grouping by the provided key selector
        /// </summary>
        public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> collection, Func<T, K> keySelector)
        {
            return collection.GroupBy(x => keySelector(x)).Select(x => x.First());
        }

        /// <summary>
        /// Synchronizes a souce collection with a destination collection using the provided: equality comparer,
        /// constructor, and updater Func's.
        /// </summary>
        /// <param name="equalityComparer">Anonymous method that provides a bool for the source / dest inputs</param>
        /// <param name="construct">Anonymous method that constructs a TDest object</param>
        /// <param name="update">Anonymous method that updates a destination object from the source</param>
        public static void SynchronizeFrom<TSource, TDest>(
                            this IList<TDest> destCollection,
                            IEnumerable<TSource> sourceCollection,
                            Func<TSource, TDest, bool> equalityComparer,
                            Func<TSource, TDest> construct,
                            Action<TSource, TDest> update)
        {
            foreach (var item in sourceCollection)
            {
                var itemGridRowItem = destCollection.FirstOrDefault(x => equalityComparer(item, x));

                // Add
                if (itemGridRowItem == null)
                    destCollection.Add(construct(item));

                // Update
                else
                    update(item, itemGridRowItem);
            }
            for (int i = destCollection.Count - 1; i >= 0; i--)
            {
                // Remove
                if (!sourceCollection.Any(x => equalityComparer(x, destCollection[i])))
                    destCollection.RemoveAt(i);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Common.Extension
{
    public static class EnumerableExtension
    {
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
        /// Filters out some elements of a list
        /// </summary>
        /// <param name="filter">filter to match elements by</param>
        public static void Filter<T>(this IList<T> list, Func<T, bool> filter)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (filter(list[i]))
                    list.RemoveAt(i);
            }
        }

        /// <summary>
        /// Forces actualization of Lazy enumerables
        /// </summary>
        public static IEnumerable<T> Actualize<T>(this IEnumerable<T> collection)
        {
            return collection.ToList();
        }

        /// <summary>
        /// Selects a random element from the sequence using the supplied random number draw U[0,1).
        /// </summary>
        public static T PickRandom<T>(this IEnumerable<T> collection, double randomNumber)
        {
            return collection.ElementAt((int)(collection.Count() * randomNumber));
        }

        /// <summary>
        /// Copies elements either by reference or by value into a new collection
        /// </summary>
        public static IEnumerable<T> Copy<T>(this IEnumerable<T> collection)
        {
            var result = new List<T>(collection.Count());

            foreach (var item in collection)
                result.Add(item);

            return result;
        }
    }
}

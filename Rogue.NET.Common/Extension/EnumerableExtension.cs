using System;
using System.Collections.Generic;

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
    }
}

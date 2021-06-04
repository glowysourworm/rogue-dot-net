using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Allocates array and copies data using the supplied selector
        /// </summary>
        public static TResult[] Transform<T, TResult>(this T[] array, Func<T, TResult> selector)
        {
            var result = new TResult[array.Length];

            for (int index = 0; index < array.Length; index++)
                result[index] = selector(array[index]);

            return result;
        }
    }
}

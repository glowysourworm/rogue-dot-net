using Rogue.NET.Common.Collection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Extension
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// Removes items from the dictionary specified by the given filter and returns those item values
        /// </summary>
        /// <typeparam name="K">Key Type</typeparam>
        /// <typeparam name="V">Value Type</typeparam>
        /// <param name="filter">Func that specifies a filter on the supplied dictionary</param>
        /// <returns>Returns new Dictionary of filtered values</returns>
        public static SimpleDictionary<K, V> Filter<K, V>(this SimpleDictionary<K, V> dictionary, Func<KeyValuePair<K, V>, bool> filter)
        {
            var result = new SimpleDictionary<K, V>();

            var removeKeys = dictionary
                               .Where(x => filter(x))
                               .Select(x => x.Key)
                               .ToList();

            foreach (var key in removeKeys)
            {
                // Add to result
                result.Add(key, dictionary[key]);

                // Remove from dictionary
                dictionary.Remove(key);
            }

            return result;
        }

        public static SimpleDictionary<KResult, VResult> ToSimpleDictionary<K, V, KResult, VResult>(this SimpleDictionary<K, V> dictionary, 
                                                                                                     Func<KeyValuePair<K, V>, KResult> keySelector,
                                                                                                     Func<KeyValuePair<K, V>, VResult> valueSelector)
        {
            var result = new SimpleDictionary<KResult, VResult>();

            foreach (var element in dictionary)
                result.Add(keySelector(element), valueSelector(element));

            return result;
        }
    }
}

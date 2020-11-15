using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace instance.id.EATK.Extensions
{
    public static class CollectionExtensions
    {
        // ------------------------------------------------------------------- Dictionary Functions
        // -- Dictionary Functions ----------------------------------------------------------------

        #region Dictionary

        public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
        {
            dict.TryGetValue(key, out var value);
            return value;
        }

        public static T GetOrAdd<T, TKey>(this Dictionary<TKey, T> dictionary, TKey key, Func<TKey, T> func)
        {
            if (dictionary.TryGetValue(key, out var result))
            {
                return result;
            }

            result = func(key);
            dictionary.Add(key, result);
            return result;
        }

        public static bool TryAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }

        public static void TryAddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys, TValue value)
        {
            foreach (var k in keys)
            {
                dictionary.TryAddValue(k, value);
            }
        }

        /// <summary>
        /// Iterate the keys and return the length of the longest entry
        /// </summary>
        /// <param name="dictionary">The collection to iterate</param>
        /// <param name="keys">The items in which to find the desired longest entry</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns> The int value of the longest length entry</returns>
        public static int GetKeyWidth<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
        {
            var keyList = dictionary.Keys.ToList();
            // -- Using the list of strings, check the length of each and return the value of the longest one -
            return keyList
                .Select(x => x.ToString())
                .Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length;
        }

        #endregion

        // ------------------------------------------------------------------------- List Functions
        // -- List Functions ----------------------------------------------------------------------

        #region List

        /// <summary>
        /// If item does not already exist in collection, add the item and return true. Else, return false
        /// </summary>
        /// <param name="list">The collection in which to attempt to add an item</param>
        /// <param name="value">The value in which to attempt to add</param>
        /// <typeparam name="TValue">The type which this collection contains</typeparam>
        /// <returns>True if successfully added, else return false</returns>
        public static bool TryAddValue<TValue>(this List<TValue> list, TValue value)
        {
            if (list.Contains(value)) return false;
            list.Add(value);
            return true;
        }

        /// <summary>
        /// If item exists in collection, return the item
        /// </summary>
        /// <param name="list">The collection in which to attempt to retrieve item</param>
        /// <param name="value">The name of the item in which to retrieve</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns>If desired item is found in list, return the item</returns>
        public static TValue TryGetValue<TValue>(this List<TValue> list, string value) where TValue : Object
        {
            return list.FirstOrDefault(x => x.name == value);
        }

        /// <summary>
        /// Iterate the list values and return the length of the longest entry
        /// </summary>
        /// <param name="list">The collection to iterate</param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns> The int value of the longest length entry</returns>
        public static int GetIntWidth<TValue>(this List<TValue> list)
        {
            // -- Using the list of strings, check the length of each and return the value of the longest one -
            return list
                .Select(x => x.ToString())
                .Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length;
        }

        // -- Iteration --------------------------------------------------
        public static IEnumerable<T> ForEachR<T>(this List<T> source, Action<T> action, bool reverse = false)
        {
            var forEach = source as T[] ?? source.ToArray();

            if (reverse)
                for (var i = forEach.Length - 1; i >= 0; i--)
                {
                    action(forEach[i]);
                }

            else
                foreach (var item in forEach)
                    action(item);

            return forEach;
        }

        #endregion
    }
}

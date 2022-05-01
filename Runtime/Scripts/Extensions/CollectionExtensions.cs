using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace instance.id.EATK
{
    public static class CollectionExtension
    {
        // ------------------------------------------------------------------- Dictionary Functions
        // -- Dictionary Functions ----------------------------------------------------------------

        #region Dictionary

        public static Tvalue TryGetVal<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
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

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }

        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
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
                dictionary.TryAdd(k, value);
            }
        }

        #region Removal

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, ref TKey key, out Exception any)
        {
            any = null;

            try
            {
                //Attempt to remove
                return dictionary.Remove(key);
            }
            catch (Exception ex)
            {
                //Assign the exception
                any = ex;

                //Indicate if a failure occured
                return false;
            }
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, ref TKey key, out TValue value, out Exception any)
        {
            any = null;

            value = default(TValue);

            try
            {
                //Attempt to remove if contaioned
                if (dictionary.TryGetValue(key, out value))
                {
                    return TryRemove(dictionary, key, out any);
                }

                //The item was not contained
                return false;
            }
            catch (Exception ex)
            {
                //Assign the exception
                any = ex;

                //Indicate if a failure occured
                return false;
            }
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, out Exception any)
        {
            return TryRemove(dictionary, ref key, out any);
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, ref TKey key)
        {
            Exception any;

            return TryRemove(dictionary, ref key, out any);
        }

        #endregion

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

        public static bool TryAdd<TValue>(this List<TValue> list, TValue value)
        {
            if (list.Contains(value)) return false;
            list.Add(value);
            return true;
        }

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
        /// <summary>
        /// Reverse ForEach. Same as the usual one, just.. the other way.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <param name="reverse"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

        public static void forEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            int i = 0;
            foreach (var e in ie)
            {
                action(e, i);
                i++;
            }
        }

        #endregion
    }
}

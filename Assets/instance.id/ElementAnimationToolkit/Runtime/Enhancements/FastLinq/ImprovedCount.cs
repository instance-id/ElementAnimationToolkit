using System;
using System.Collections.Generic;

namespace FastLinq
{
    public static partial class ImprovedLinq
    {
        public static int Count<TSource>(this TSource[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Length;
        }

        public static int Count<TSource>(
        this TSource[] source,
        Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int count = 0;
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    count++;
                }
            }

            return count;
        }

        public static int Count<TSource>(this List<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Count;
        }

        public static int Count<TSource>(
        this List<TSource> source,
        Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int count = 0;
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    count++;
                }
            }

            return count;
        }

        public static int Count<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Count;
        }

        public static int Count<TKey, TValue>(this Dictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            int count = 0;
            foreach (var pair in source)
            {
                if (predicate(pair))
                {
                    count++;
                }
            }

            return count;
        }
    }
}

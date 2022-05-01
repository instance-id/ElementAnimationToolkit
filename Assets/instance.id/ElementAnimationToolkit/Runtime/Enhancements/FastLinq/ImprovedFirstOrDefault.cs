using System;
using System.Collections.Generic;

namespace FastLinq
{
    public static partial class ImprovedLinq
    {
        public static TSource FirstOrDefault<TSource>(this TSource[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Length == 0) return default;
            return source[0];
        }

        public static TSource FirstOrDefault<TSource>(this List<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) return default;
            return source[0];
        }

        public static KeyValuePair<TKey, TSource> FirstOrDefault<TKey, TSource>(this Dictionary<TKey, TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) return default;
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
            return default;
        }

        public static TSource FirstOrDefault<TSource>(this TSource[] source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            foreach (TSource element in source)
            {
                if (predicate(element)) return element;
            }

            return default;
        }

        public static TSource FirstOrDefault<TSource>(this List<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            foreach (TSource element in source)
            {
                if (predicate(element)) return element;
            }

            return default;
        }

        public static KeyValuePair<TKey, TSource> FirstOrDefault<TKey, TSource>(this Dictionary<TKey, TSource> source,
        Func<KeyValuePair<TKey, TSource>, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            foreach (var pair in source)
            {
                if (predicate(pair)) return pair;
            }

            return default;
        }
    }
}

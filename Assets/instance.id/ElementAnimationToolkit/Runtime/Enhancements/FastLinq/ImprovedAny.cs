using System;
using System.Collections.Generic;

namespace FastLinq
{
    public static partial class ImprovedLinq
    {
        public static bool Any<TSource>(this TSource[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Length > 0)
            {
                return true;
            }

            return false;
        }

        public static bool Any<TSource>(this List<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count > 0)
            {
                return true;
            }

            return false;
        }

        public static bool Any<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count > 0)
            {
                return true;
            }

            return false;
        }

        public static bool Any<TSource>(
        this TSource[] source,
        Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Any<TSource>(
        this List<TSource> source,
        Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Any<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

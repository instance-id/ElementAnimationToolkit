using System;
using System.Collections.Generic;

namespace FastLinq
{
    public static partial class ImprovedLinq
    {
        public static T LastOrDefault<T>(this T[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Length == 0) return default;
            return source[source.Length - 1];
        }

        public static T LastOrDefault<T>(this T[] source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            for (var i = source.Length - 1; i >= 0; i--)
            {
                if (predicate(source[i]))
                {
                    return source[i];
                }
            }

            return default;
        }

        public static T LastOrDefault<T>(this List<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) return default;
            return source[source.Count - 1];
        }

        public static T LastOrDefault<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            for (var i = source.Count - 1; i >= 0; i--)
            {
                if (predicate(source[i]))
                {
                    return source[i];
                }
            }

            return default;
        }

        public static KeyValuePair<TKey, TValue> LastOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) return default;
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    KeyValuePair<TKey, TValue> result;
                    do
                    {
                        result = e.Current;
                    } while (e.MoveNext());

                    return result;
                }
            }

            return default;
        }

        public static KeyValuePair<TKey, TValue> LastOrDefault<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) return default;
            bool found = false;
            KeyValuePair<TKey, TValue> result = default;
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    do
                    {
                        if (predicate(e.Current))
                        {
                            result = e.Current;
                            found = true;
                        }
                    } while (e.MoveNext());

                }
            }

            if (found)
            {
                return result;
            }

            return default;
        }
    }
}

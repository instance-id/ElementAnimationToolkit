using System;
using System.Collections.Generic;

namespace FastLinq
{
    public static partial class ImprovedLinq
    {
        public static T Last<T>(this T[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Length == 0) throw new InvalidOperationException();
            return source[source.Length - 1];
        }

        public static T Last<T>(this T[] source, Func<T, bool> predicate)
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

            throw new InvalidOperationException();
        }

        public static T Last<T>(this List<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) throw new InvalidOperationException();
            return source[source.Count - 1];
        }

        public static T Last<T>(this List<T> source, Func<T, bool> predicate)
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

            throw new InvalidOperationException();
        }

        public static KeyValuePair<TKey, TValue> Last<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) throw new InvalidOperationException();
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

            throw new InvalidOperationException();
        }

        public static KeyValuePair<TKey, TValue> Last<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) throw new InvalidOperationException();
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

            throw new InvalidOperationException();
        }
    }
}

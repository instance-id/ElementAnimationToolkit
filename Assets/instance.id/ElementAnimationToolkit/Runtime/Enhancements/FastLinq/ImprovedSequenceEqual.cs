using System;
using System.Collections.Generic;

namespace FastLinq
{
    public static partial class ImprovedLinq
    {
        public static bool SequenceEqual<TSource>(
        this TSource[] first,
        TSource[] second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (first.Length != second.Length) return false;
            return SequenceEqual(first, second, null);
        }

        public static bool SequenceEqual<TSource>(
        this TSource[] first,
        TSource[] second,
        IEqualityComparer<TSource> comparer)
        {
            if (comparer == null) comparer = EqualityComparer<TSource>.Default;
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (first.Length != second.Length) return false;
            var length = first.Length;
            for (var i = 0; i < length; i++)
            {
                if (!comparer.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool SequenceEqual<TSource>(
        this List<TSource> first,
        List<TSource> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (first.Count != second.Count) return false;
            return SequenceEqual(first, second, null);
        }

        public static bool SequenceEqual<TSource>(
        this List<TSource> first,
        List<TSource> second,
        IEqualityComparer<TSource> comparer)
        {
            if (comparer == null) comparer = EqualityComparer<TSource>.Default;
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (first.Count != second.Count) return false;
            var length = first.Count;
            for (var i = 0; i < length; i++)
            {
                if (!comparer.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}

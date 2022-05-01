using System;
using System.Collections.Generic;

namespace FastLinq
{
// NOTE
// Contains(this IEnumerable<TSource> source, TSource value);
// この場合は、内部的にはICollectionであるかを判定して、該当する場合はICollection.Containsを呼び出す
// Arrayクラスの場合は、ICollection.Containsの実装でTrySZIndexOfを使っており、プリミティブタイプに最適化されたnative method呼び出しでかなり速い
// C#レイヤーでforeachの最適化でゴニョるより、素直に呼び出す方が速いので、ここではIEqualityComparerを指定する場合のみの最適化を行なっている
    public static partial class ImprovedLinq
    {
        public static bool Contains<TSource>(
        this TSource[] source,
        TSource value,
        IEqualityComparer<TSource> comparer)
        {
            if (comparer == null) comparer = EqualityComparer<TSource>.Default;
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
            {
                if (comparer.Equals(element, value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains<TSource>(
        this List<TSource> source,
        TSource value,
        IEqualityComparer<TSource> comparer)
        {
            if (comparer == null) comparer = EqualityComparer<TSource>.Default;
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
            {
                if (comparer.Equals(element, value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        KeyValuePair<TKey, TValue> value,
        IEqualityComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            if (comparer == null) comparer = EqualityComparer<KeyValuePair<TKey, TValue>>.Default;
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var pair in source)
            {
                if (comparer.Equals(pair, value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

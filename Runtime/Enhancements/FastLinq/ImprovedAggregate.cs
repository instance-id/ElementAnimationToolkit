using System;
using System.Collections.Generic;

namespace FastLinq
{
    public static partial class ImprovedLinq
    {
        public static TResult Aggregate<TSource, TAccumulate, TResult>(
        this TSource[] source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func,
        Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            TAccumulate result = seed;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return resultSelector(result);
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(
        this TSource[] source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            TAccumulate result = seed;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return result;
        }

        public static TSource Aggregate<TSource>(this TSource[] source, Func<TSource, TSource, TSource> func)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            TSource result = default;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return result;
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(
        this List<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func,
        Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            TAccumulate result = seed;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return resultSelector(result);
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(
        this List<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            TAccumulate result = seed;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return result;
        }

        public static TSource Aggregate<TSource>(
        this List<TSource> source,
        Func<TSource, TSource, TSource> func)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            TSource result = default;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return result;
        }

        public static TResult Aggregate<TKey, TValue, TAccumulate, TResult>(
        this Dictionary<TKey, TValue> source,
        TAccumulate seed,
        Func<TAccumulate, KeyValuePair<TKey, TValue>, TAccumulate> func,
        Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            TAccumulate result = seed;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return resultSelector(result);
        }

        public static TAccumulate Aggregate<TKey, TValue, TAccumulate>(
        this Dictionary<TKey, TValue> source,
        TAccumulate seed,
        Func<TAccumulate, KeyValuePair<TKey, TValue>, TAccumulate> func)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            TAccumulate result = seed;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return result;
        }

        public static KeyValuePair<TKey, TValue> Aggregate<TKey, TValue>(
        this Dictionary<TKey, TValue> source,
        Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> func)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (func == null) throw new ArgumentNullException(nameof(func));
            KeyValuePair<TKey, TValue> result = default;
            foreach (var element in source)
            {
                result = func(result, element);
            }

            return result;
        }
    }
}

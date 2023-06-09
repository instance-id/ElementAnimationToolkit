// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/Extensions                    --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;

namespace instance.id.EATK.Extensions
{
    // -- https://github.com/TeamSirenix/odin-serializer/blob/ecd5ee6cf3d71b654b62926f85e699a3db47d9f6/OdinSerializer/Utilities/Extensions/LinqExtensions.cs
    public static class LinqExtensions
    {
        [SourceTemplate]
        public static void forEach<T>(this IEnumerable<T> z)
        {
            z.ForEach(x =>
            {
                //$ $END$
            });
        }
        
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            var forEach = source as T[] ?? source.ToArray();
            foreach (var item in forEach) action(item);
            return forEach;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var counter = 0;
            var forEach = source as T[] ?? source.ToArray();
            foreach (var item in forEach) action(item, counter++);
            return forEach;
        }
        
        public static T[] ForEach<T>(this T[] source, Action<T> action)
        {
            foreach (var item in source) action(item);
            return source;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, IEnumerable<T> append)
        {
            foreach (var item in source) yield return item;
            foreach (var item in append) yield return item;
        }


        // https://gist.github.com/neremin/dd6f69974786babeba8c ---------------------------
        /// <devdoc>
        /// Y-Combinator for Lambda recursion.
        /// Based on http://www.codeproject.com/Articles/68978/Recursively-Traversing-a-Hierarchical-Data-Structu?msg=3435479#xx3435479xx
        /// </devdoc>
        static Func<A, A> Y<A>(Func<Func<A, A>, Func<A, A>> F)
        {
            return a => F(Y(F))(a);
        }

        static IEnumerable<T> SelectSubTreeIterator<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Predicate<T> canSelect)
        {
            foreach (T element in source)
            {
                if (canSelect(element))
                {
                    yield return element;
                    foreach (T subElement in selector(element))
                    {
                        yield return subElement;
                    }
                }
            }
        }

        /// <summary>
        /// Traverse items tree and returns items matching <paramref name="canSelect"/>.
        /// </summary>
        /// <remarks>
        /// If node does not match <paramref name="canSelect"/> this node sub-tree is skipped.
        /// </remarks>
        /// <typeparam name="T">Type of tree node.</typeparam>
        /// <param name="source">Root nodes collection.</param>
        /// <param name="selector">Returns children of the given item.</param>
        /// <paramref name="canSelect">Predicate for filtering matching nodes.</paramref>
        /// <returns>Items including <paramref name="source"/> and their sub-trees matching <paramref name="canSelect"/>.</returns>
        /// <remarks>
        /// Search enabled controls by Name:
        /// <code>
        /// Control root;
        /// root.Controls
        ///       .Cast&lt;Control&gt;()
        ///       .Traverse
        ///        (
        ///             c => c.Controls.Cast&lt;Control&gt;(),
        ///             c => c.Enabled
        ///        )
        ///       .Where(c => c.Name == "Searched name");
        /// </code>
        /// </remarks>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Predicate<T> canSelect)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Requires<ArgumentNullException>(canSelect != null);

            return Y<IEnumerable<T>>
                (
                    f => items => SelectSubTreeIterator(items, item => f(selector(item)), canSelect)
                )
                (source);
        }

        /// <summary>
        /// Traverse items tree and returns items matching <paramref name="canSelect"/>.
        /// </summary>
        /// <remarks>
        /// If node does not match <paramref name="canSelect"/> this node sub-tree is skipped.
        /// </remarks>
        /// <typeparam name="T">Type of tree node.</typeparam>
        /// <param name="root">Tree root node.</param>
        /// <param name="selector">Returns children of the given item.</param>
        /// <paramref name="canSelect">Predicate for filtering matching nodes.</paramref>
        /// <returns>Sub-trees nodes matching <paramref name="canSelect"/>, except <paramref name="root"/>.</returns>
        /// <remarks>
        /// Search enabled controls by Name:
        /// <code>
        /// Control root;
        /// root.Traverse
        ///      (
        ///             c => c.Controls.Cast&lt;Control&gt;(),
        ///             c => c.Enabled
        ///      )
        ///     .Where(c => c.Name == "Searched name");
        /// </code>
        /// </remarks>
        public static IEnumerable<T> Traverse<T>(this T root, Func<T, IEnumerable<T>> selector, Predicate<T> canSelect)
        {
            Contract.Requires<ArgumentNullException>(root != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            return selector(root).Traverse(selector, canSelect);
        }

        /// <summary>
        /// Traverse items tree and returns all items.
        /// </summary>
        /// <typeparam name="T">Type of tree node.</typeparam>
        /// <param name="source">Root nodes collection.</param>
        /// <param name="selector">Returns children of the given item.</param>
        /// <returns>All items in the tree, including <paramref name="source"/>.</returns>
        /// <remarks>
        /// Search all controls by Name:
        /// <code>
        /// Control root;
        /// root.Controls
        ///       .Cast&lt;Control&gt;()
        ///       .Traverse
        ///        (
        ///             c => c.Controls.Cast&lt;Control&gt;(),
        ///        )
        ///       .Where(c => c.Name == "Searched name");
        /// </code>
        /// </remarks>
        public static IEnumerable<T> TraverseAll<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            return source.Traverse(selector, e => true);
        }

        /// <summary>
        /// Traverse items tree and returns all items.
        /// </summary>
        /// <typeparam name="T">Type of tree node.</typeparam>
        /// <param name="root">Tree root node.</param>
        /// <param name="selector">Returns children of the given item.</param>
        /// <returns>All items except <paramref name="root"/>.</returns>
        /// <remarks>
        /// Search all controls by Name:
        /// <code>
        /// Control root;
        /// root.Traverse
        ///      (
        ///           c => c.Controls.Cast&lt;Control&gt;(),
        ///      )
        ///     .Where(c => c.Name == "Searched name");
        /// </code>
        /// </remarks>
        public static IEnumerable<T> TraverseAll<T>(this T root, Func<T, IEnumerable<T>> selector)
        {
            return root.Traverse(selector, e => true);
        }

        /// <summary>
        /// Prepends element to the sequence.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="tail">Appended sequence.</param>
        /// <param name="head">Prepending element.</param>
        /// <returns>Concatenated sequence with the <paramref name="head"/> as the first element.</returns>
        /// <remarks>
        /// <code>
        /// Enumrable.Empty&lt;int&gt;().Prepend(10);
        /// </code>
        /// </remarks>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> tail, T head)
        {
            yield return head;
            foreach (var item in tail)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Appends element to the sequence.
        /// </summary>
        /// <typeparam name="T">Type of sequence elements.</typeparam>
        /// <param name="tail">Appended element.</param>
        /// <param name="head">Prepending elements sequence.</param>
        /// <returns>Concatenated sequence with the <paramref name="tail"/> as the last element.</returns>
        /// <remarks>
        /// <code>
        /// Enumrable.Empty&lt;int&gt;().Append(10);
        /// </code>
        /// </remarks>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> head, T tail)
        {
            foreach (var item in head)
            {
                yield return item;
            }

            yield return tail;
        }

        /// <summary>
        /// Converts sequence into HashSet.
        /// </summary>
        /// <typeparam name="T">Time of sequence element.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <returns><see cref="HashSet{T}"/> instance, initialized with the seqence elements.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type <see cref="String"/>.
        /// </summary>
        /// <param name="values">A collection that contains the strings to concatenate.</param>
        /// <returns>The concatenated strings in values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        public static string Concat(this IEnumerable<string> values)
        {
            var array = values as string[];
            return array == null ? string.Concat(values) : string.Concat(array);
        }

        /// <summary>
        /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type <see cref="String"/>,
        /// using the specified separator between each member.
        /// </summary>
        /// <param name="values">A collection that contains the strings to concatenate.</param>
        /// <param name="separator">The string to use as a separator.</param>
        /// <returns>A string that consists of the members of values delimited by the separator string.
        /// If values has no members, the method returns <see cref="String.Empty"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        public static string Join(this IEnumerable<string> values, string separator)
        {
            var array = values as string[];
            return array == null ? string.Join(separator, values) : string.Join(separator, array);
        }

        /// <summary>
        /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type <see cref="String"/>,
        /// using <see cref="Environment.NewLine"/> between each member.
        /// </summary>
        /// <param name="lines">A collection that contains the strings to concatenate.</param>
        /// <returns>A string that consists of the members of values delimited by the <see cref="Environment.NewLine"/>.
        /// If values has no members, the method returns <see cref="String.Empty"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="lines"/> is null.</exception>
        public static string JoinLines(this IEnumerable<string> lines)
        {
            return lines.Join(Environment.NewLine);
        }

        /// <summary>
        /// Select projection of two sibling items in a sequence pairwise.
        /// </summary>
        /// <typeparam name="TSource">Source sequence item type</typeparam>
        /// <typeparam name="TResult">Projected item type</typeparam>
        /// <param name="source">Source items sequence.</param>
        /// <param name="selector">Projects previous and current item to a new value.</param>
        /// <returns>Sequence of projected sibling pairs.</returns>
        /// <remarks>
        /// Numbers sequence to deltas:
        /// <code>
        /// var deltas = Enumerable.Range(0, 10000).SelectPairwise((first, second) => second - first)
        /// </code>
        /// In .NET 4.0+
        /// <code>
        /// var sequence = Enumerable.Range(0, 10000).ToArray();
        /// var deltas = sequence.Zip(sequence.Skip(1), (first, second) => second - first);
        /// </code>
        /// </remarks>
        /// <devdoc>
        /// Based on http://stackoverflow.com/a/3970131/550068 and http://stackoverflow.com/a/3683217/550068
        /// </devdoc>
        public static IEnumerable<TResult> SelectPairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> selector)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    yield break; // throw new InvalidOperationException("Sequence cannot be empty.");
                }

                var previous = e.Current;
                if (!e.MoveNext())
                {
                    yield break; // throw new InvalidOperationException("Sequence must contain at least two elements.");
                }

                do
                {
                    yield return selector(previous, e.Current);
                    previous = e.Current;
                } while (e.MoveNext());
            }
        }

        public static IEnumerable<object> forEach(this IEnumerable<object> source, Action<object> action)
        {
            var enumerable = source as object[] ?? source.ToArray();
            foreach (var item in enumerable) action(item);
            return enumerable;
        }
    }
}

// License of SelectRecursive portions: CPOL
//  - retrieved from http://www.codeproject.com/Tips/80746/Select-Recursive-Extension-method on July 11, 2012

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LionFire
{
    /// <summary>
    /// Extension methods for <see cref="T:System.Collections.Generic.IEnumerable`1"/> 
    /// </summary>
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item) => enumerable.Concat(new T[] { item });

        // See SelectRecursive in Linq 

        // FIXME - actually recurse using SelectRecursive
        public static IEnumerable SelectRecursive(this IEnumerable source)
        {
            foreach (var directChild in source)
            {
                yield return directChild;
                if (directChild is IEnumerable enumerableChild)
                {
                    foreach (var descendant in enumerableChild)
                    {
                        yield return descendant;
                    }
                }
            }
        }

        // FIXME - actually recurse using SelectRecursive
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source)
        {
            foreach (T directChild in source)
            {
                yield return directChild;
                if (directChild is IEnumerable<T> enumerableChild)
                {
                    foreach (T descendant in enumerableChild)
                    {
                        yield return descendant;
                    }
                }
            }
        }

        // REVIEW - SelectMany



        //public static IEnumerable<T> SelectRecursive<T>(this T source, bool yieldTopItem = true)
        //    where T : IEnumerable<T>
        //{
        //    if (yieldTopItem) { yield return source; }
        //    foreach (T directChild in source)
        //    {
        //        foreach (var recurseChild in directChild.SelectRecursive())
        //        {
        //            yield return recurseChild;
        //        }
        //    }
        //}

        //public static IEnumerable<T> SelectRecursive<T>(this T source, Func<T, IEnumerable<T>> getChildren, bool yieldTopItem = true)
        //{
        //    if (yieldTopItem) { yield return source; }
        //    foreach (T directChild in getChildren(source))
        //    {
        //        foreach (var recurseChild in directChild.SelectRecursive(getChildren))
        //        {
        //            yield return recurseChild;
        //        }
        //    }
        //}

        #region SelectRecursive

#if AOT
		
		/// <summary>
		/// Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> 
		/// and flattens the resulting sequences into one sequence. 
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="source">A sequence of values to project.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>
		/// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements 
		/// who are the result of invoking the recursive transform function on each element of the input sequence. 
		/// </returns>
		/// <example>
		/// node.ChildNodes.SelectRecursive(n => n.ChildNodes);
		/// </example>
		public static IEnumerable SelectRecursive(this IEnumerable source, Func<object, IEnumerable> selector)
		{
			return SelectRecursive(source, selector, null);
		}

		/// <summary>
		/// Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> 
		/// and flattens the resulting sequences into one sequence. 
		/// </summary>
		/// <typeparam name="T">The type of the elements.</typeparam>
		/// <param name="source">A sequence of values to project.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <param name="predicate">A function to test each element for a condition in each recursion.</param>
		/// <returns>
		/// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of 
		/// invoking the recursive transform function on each element of the input sequence. 
		/// </returns>
		/// <example>
		/// node.ChildNodes.SelectRecursive(n => n.ChildNodes, m => m.Depth < 2);
		/// </example>
		public static IEnumerable SelectRecursive(this IEnumerable source, Func<object, IEnumerable> selector, Func<Recursion, bool> predicate)
		{
			return SelectRecursive(source, selector, predicate, 0);
		}

		private static IEnumerable<Recursion> SelectRecursive(IEnumerable source, Func<object, IEnumerable> selector, Func<Recursion, bool> predicate, int depth)
		{
			List<Recursion> list = new List<Recursion>();
			foreach(var x in source) {
				var rec = new Recursion(depth, selector(x));
				if((predicate == null || predicate(rec)))
			  {
					list.Add(rec);
				}
			}

			foreach (var item in list)
			{
				yield return item;
				foreach (var item2 in SelectRecursive(selector(item.Item), selector, predicate, depth + 1))
					yield return item2;
			}
		}
#else

        /// <summary>
        /// Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> 
        /// and flattens the resulting sequences into one sequence. 
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements 
        /// who are the result of invoking the recursive transform function on each element of the input sequence. 
        /// </returns>
        /// <example>
        /// node.ChildNodes.SelectRecursive(n => n.ChildNodes);
        /// </example>
        public static IEnumerable<IRecursion<T>> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector) => SelectRecursive(source, selector, null);

        /// <summary>
        /// Projects each element of a sequence recursively to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> 
        /// and flattens the resulting sequences into one sequence. 
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <param name="predicate">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of 
        /// invoking the recursive transform function on each element of the input sequence. 
        /// </returns>
        /// <example>
        /// node.ChildNodes.SelectRecursive(n => n.ChildNodes, m => m.Depth < 2);
        /// </example>
        public static IEnumerable<IRecursion<T>> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Func<IRecursion<T>, bool> predicate) => SelectRecursive(source, selector, predicate, 0);

        private static IEnumerable<IRecursion<T>> SelectRecursive<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> selector, Func<IRecursion<T>, bool> predicate, int depth)
        {
            IEnumerable<IRecursion<T>> q = source
                .Select(item => new Recursion<T>(depth, item))
                .Cast<IRecursion<T>>();
            if (predicate != null)
                q = q.Where(predicate);
            foreach (IRecursion<T> item in q)
            {
                yield return item;
                foreach (IRecursion<T> item2 in SelectRecursive(selector(item.Item), selector, predicate, depth + 1))
                    yield return item2;
            }
        }
#endif

#if AOT
		public class Recursion
		{
			private int _depth;
			private object _item;
			public int Depth
			{
				get { return _depth; }
			}
			public Object Item
			{
				get { return _item; }
			}
			public Recursion(int depth, object item)
			{
				_depth = depth;
				_item = item;
			}
		}
#else

        private class Recursion<T>
#if !AOT
            : IRecursion<T>
#endif
        {
            private readonly int _depth;
            private readonly T _item;
            public int Depth => _depth;
            public T Item => _item;
            public Recursion(int depth, T item)
            {
                _depth = depth;
                _item = item;
            }
        }
#endif
        #endregion
    }

    #region SelectRecursive support
#if !AOT
    /// <summary>
    /// Represents an item in a recursive projection.
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    public interface IRecursion<T>
    {
        /// <summary>
        /// Gets the recursive depth.
        /// </summary>
        /// <value>The depth.</value>
        int Depth { get; }
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        T Item { get; }
    }
#endif
    #endregion

}

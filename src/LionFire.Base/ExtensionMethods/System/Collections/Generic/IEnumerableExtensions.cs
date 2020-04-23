using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.ExtensionMethods
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item) // RENAME to ToEnumerable?
        {
            yield return item;
        }

        /// <summary>
        /// Perform the action for each item in the enumerable
        /// </summary>
        /// <typeparam name="T">Type of the IEnumerable</typeparam>
        /// <param name="enumerable">Enumerable to apply action to</param>
        /// <param name="action">Action to perform on each item of enumerable</param>
        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) { action(item); }
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> enumerable) => enumerable ?? Enumerable.Empty<T>();

    }
}
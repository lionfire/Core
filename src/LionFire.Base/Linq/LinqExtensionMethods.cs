using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class LinqExtensionMethods
    {
        public static bool NullableAny<T>(this IEnumerable<T> enumerable) 
            => enumerable == null ? false : enumerable.Any();

        public static T AggregateOrDefault<T>(this IEnumerable<T> enumerable, Func<T, T, T> aggregator) 
            => !enumerable.Any() ? (default) : enumerable.Aggregate(aggregator);

        public static T AggregateOrDefault<T>(this IEnumerable<T> enumerable, Func<T, T, T> aggregator, T defaultValue) 
            => !enumerable.Any() ? defaultValue : enumerable.Aggregate(aggregator);
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{

    public static class RepeatAllUntilTrueExtensions
    {
        public static Task RepeatAllUntilTrue<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<bool>>> getResultForItem, CancellationToken cancellationToken = default, bool parallel = false)
            => items.RepeatAllUntil<TItem, bool>(getResultForItem, r => r, cancellationToken, parallel);

        public static Task<(bool succeeded, IEnumerable<KeyValuePair<TItem, bool>> remainingItems)> TryRepeatAllUntilTrue<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<bool>>> getResultForItem, CancellationToken cancellationToken = default, bool parallel = false)
            => items.TryRepeatAllUntil(getResultForItem, r => r, cancellationToken, parallel);
    }

    public static class RepeatAllUntilNullExtensions
    {
        public static Task RepeatAllUntilNull<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<object>>> getResultForItem, CancellationToken cancellationToken = default, bool parallel = false)
            => items.RepeatAllUntil(getResultForItem, r => r == null, cancellationToken, parallel);

        public static Task<(bool succeeded, IEnumerable<KeyValuePair<TItem, object>> remainingItems)> TryRepeatAllUntilNull<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<object>>> getResultForItem, CancellationToken cancellationToken = default, bool parallel = false)
            => items.TryRepeatAllUntil(getResultForItem, r => r == null, cancellationToken, parallel);

    }
}

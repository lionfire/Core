using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public static class RepeatAllUntilExtensions
    {
        public static async Task RepeatAllUntil<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, Func<Task<TResult>>> getResultForItem, Predicate<TResult> resultIsSuccessful, bool parallel = false)
        {
            (bool succeeded, IEnumerable<KeyValuePair<TItem, TResult>> remaining) = await TryRepeatAllUntil<TItem, TResult>(items, getResultForItem, resultIsSuccessful, parallel);
            if (!succeeded)
            {
                string msg;
                try
                {
                    msg = $"No progress made on executing {remaining} remaining items: " + remaining.Select(c => c.Key.ToString()).Aggregate((x, y) => x + ", " + y);
                }
                catch
                {
                    msg = $"No progress made on executing {remaining} remaining items.";
                }
                throw new Exception(msg);
            }
        }

        public static async Task<(bool succeeded, IEnumerable<KeyValuePair<TItem, TResult>> remainingItems)> TryRepeatAllUntil<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, Func<Task<TResult>>> getResultForItem, Predicate<TResult> resultIsSuccessful, bool parallel = false)
        {
            if (!items.Any()) return (true, Enumerable.Empty<KeyValuePair<TItem, TResult>>());

            var needsTrue = items.Select(i => new KeyValuePair<TItem, TResult>(i, default)).ToList();
            int itemsRequiringTrue = needsTrue.Count;
            List<KeyValuePair<TItem, TResult>> stillNeedsTrue = null;

            do
            {
                if (stillNeedsTrue != null)
                {
                    needsTrue = stillNeedsTrue;
                }
                stillNeedsTrue = new List<KeyValuePair<TItem, TResult>>();

                if (parallel)
                {
                    // OPTIMIZE: instead of lock, add non-null results to stillNeedsTrue
                    object collectionLock = new object();
                    var tasks = needsTrue.Select(async item =>
                    {
                        var result = await getResultForItem(item.Key)().ConfigureAwait(false);
                        if (!resultIsSuccessful(result))
                        {
                            lock (collectionLock)
                            {
                                stillNeedsTrue.Add(new KeyValuePair<TItem, TResult>(item.Key, result));
                            }
                        }
                    });
                    await Task.WhenAll(tasks);
                }
                else
                {
                    foreach (var item in needsTrue)
                    {
                        var result = await getResultForItem(item.Key)().ConfigureAwait(false);

                        if (!resultIsSuccessful(result))
                        {
                            stillNeedsTrue.Add(new KeyValuePair<TItem, TResult>(item.Key, result));
                        }
                    }
                }

                if (stillNeedsTrue.Count == itemsRequiringTrue)
                {
                    // No progress made
                    return (false, stillNeedsTrue);
                }
                itemsRequiringTrue = stillNeedsTrue.Count;

            } while (stillNeedsTrue.Count > 0);
            return (true, Enumerable.Empty<KeyValuePair<TItem, TResult>>());
        }
    }

    public static class RepeatAllUntilTrueExtensions
    {
        public static Task RepeatAllUntilTrue<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<bool>>> getResultForItem, bool parallel = false)
            => items.RepeatAllUntil<TItem, bool>(getResultForItem, r => r, parallel);

        public static Task<(bool succeeded, IEnumerable<KeyValuePair<TItem, bool>> remainingItems)> TryRepeatAllUntilTrue<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<bool>>> getResultForItem, bool parallel = false)
            => items.TryRepeatAllUntil(getResultForItem, r => r, parallel);
    }

    public static class RepeatAllUntilNullExtensions
    {
        public static Task RepeatAllUntilNull<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<object>>> getResultForItem, bool parallel = false)
            => items.RepeatAllUntil(getResultForItem, r => r == null, parallel);

        public static Task<(bool succeeded, IEnumerable<KeyValuePair<TItem, object>> remainingItems)> TryRepeatAllUntilNull<TItem>(this IEnumerable<TItem> items, Func<TItem, Func<Task<object>>> getResultForItem, bool parallel = false)
            => items.TryRepeatAllUntil(getResultForItem, r => r == null, parallel);

    }
}

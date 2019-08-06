using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public static class RepeatAllUntilExtensions
    {
        public static async Task RepeatAllUntil<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, Func<Task<TResult>>> getResultForItem, Predicate<TResult> resultIsSuccessful, CancellationToken cancellationToken = default, bool parallel = false)
        {
            (bool succeeded, IEnumerable<KeyValuePair<TItem, TResult>> remaining) = await TryRepeatAllUntil<TItem, TResult>(items, getResultForItem, resultIsSuccessful, cancellationToken, parallel);
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="items"></param>
        /// <param name="getResultForItem"></param>
        /// <param name="resultIsSuccessful"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="parallel"></param>
        /// <returns>remainingItems may be null if canceled</returns>
        public static async Task<(bool succeeded, IEnumerable<KeyValuePair<TItem, TResult>> remainingItems)> TryRepeatAllUntil<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, Func<Task<TResult>>> getResultForItem, Predicate<TResult> resultIsSuccessful, CancellationToken cancellationToken = default, bool parallel = false)
        {
            if (!items.Any()) return (true, Enumerable.Empty<KeyValuePair<TItem, TResult>>());

            var needsTrue = items.Select(i => new KeyValuePair<TItem, TResult>(i, default)).ToList();
            int itemsRequiringTrue = needsTrue.Count;
            List<KeyValuePair<TItem, TResult>> stillNeedsTrue = null;

            (bool succeeded, IEnumerable<KeyValuePair<TItem, TResult>> remainingItems) ReturnValue()
            {
                return (stillNeedsTrue.Count == 0, stillNeedsTrue);
            }

            do
            {
                if (cancellationToken.IsCancellationRequested) return ReturnValue();

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
                        if (cancellationToken.IsCancellationRequested) return (false, null);
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
}

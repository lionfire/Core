using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
    public static class RepeatAllUntilTrueExtensions
    {
        public static async Task RepeatAllUntilTrue(this IEnumerable<object> items, Func<object, Func<Task<bool>>> selector, bool parallel = false)
        {
            var (succeeded, remaining) = await TryRepeatAllUntilTrue(items, selector, parallel);
            if (!succeeded)
            {
                var msg = $"No progress made on executing {remaining} remaining items: " + remaining.Select(c => c.ToString()).Aggregate((x, y) => x + ", " + y);
                throw new Exception(msg);
            }
        }

        public static async Task<(bool succeeded, IEnumerable<object> remainingItems)> TryRepeatAllUntilTrue(this IEnumerable<object> items, Func<object, Func<Task<bool>>> selector, bool parallel = false)
        {
            if (!items.Any()) return (true, Enumerable.Empty<object>());

            var needsTrue = items.ToList();
            int itemsRequiringTrue = needsTrue.Count;
            List<object> stillNeedsTrue = null;

            do
            {
                if (stillNeedsTrue != null)
                {
                    needsTrue = stillNeedsTrue;
                }
                stillNeedsTrue = new List<object>();

                if (parallel)
                {
                    // OPTIMIZE: instead of lock, add non-null results to stillNeedsTrue
                    object collectionLock = new object();
                    var tasks = needsTrue.Select(async item =>
                    {
                        if (await selector(item)().ConfigureAwait(false) == false)
                        {
                            lock (collectionLock)
                            {
                                stillNeedsTrue.Add(item);
                            }
                        }
                    });
                    await Task.WhenAll(tasks);
                }
                else
                {
                    foreach (var item in needsTrue)
                    {
                        if (await selector(item)().ConfigureAwait(false) == false)
                        {
                            stillNeedsTrue.Add(item);
                        }
                    }
                }

                if (stillNeedsTrue.Count == itemsRequiringTrue)
                {
                    return (false, stillNeedsTrue);
                }

            } while (stillNeedsTrue.Count > 0);
            return (true, Enumerable.Empty<object>());
        }

    }
}

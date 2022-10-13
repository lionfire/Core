using System.Collections.Generic;
using System.Linq;

namespace LionFire.ExtensionMethods.Collections;

public static class EnumerableExtensions
{
    // Based on https://gist.github.com/brianmed/1a04a9c4cfe68b7c372fe7170cecf158
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int maxItems)
    {
        return items.Select((item, index) => new { item, index })
                    .GroupBy(x => x.index / maxItems)
                    .Select(g => g.Select(x => x.item));
    }
}

#if NET6_0_OR_GREATER
#nullable enable

namespace LionFire.Threading;

public static class ValueTaskEx
{
    // Inspired by: https://stackoverflow.com/a/63141544
    public static async ValueTask WhenAll(ValueTask[] source)
    {
        ArgumentNullException.ThrowIfNull(source);

        List<Exception>? exceptions = null;

        for (var i = 0; i < source.Length; i++)
        {
            try
            {
                await source[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= new(source.Length);
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null) throw new AggregateException(exceptions);
    }

    // Inspired by: https://stackoverflow.com/a/63141544
    public static async ValueTask WhenAll(IEnumerable<ValueTask> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        List<Exception>? exceptions = null;

        foreach (var t in source)
        {
            try
            {
                await t.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= new(source.Count());
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null) throw new AggregateException(exceptions);
    }

    // Based on: https://stackoverflow.com/a/63141544
    public static async ValueTask WhenAll<T>(ValueTask<T>[] source, IList<T> target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        if (source.Length != target.Count)
            throw new ArgumentException(
                "Source and target lengths must match",
                nameof(target));

        List<Exception>? exceptions = null;

        for (var i = 0; i < source.Length; i++)
        {
            try
            {
                target[i] = await source[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= new(source.Length);
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null) throw new AggregateException(exceptions);
    }
}

#endif
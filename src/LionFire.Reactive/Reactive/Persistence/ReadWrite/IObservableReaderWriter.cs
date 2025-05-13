using DynamicData;
using LionFire.Data.Async;

namespace LionFire.Reactive.Persistence;

public interface IObservableReaderWriter<TKey, TValue>
    : IObservableReader<TKey, TValue>
    , IObservableWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
}

public static class IObservableReaderWriterX
{
    public static TValue? TryGet<TKey, TValue>(this IObservableReaderWriter<TKey, TValue> rw, TKey? portfolioId)
    where TKey : notnull
    where TValue : notnull
    {
        if(portfolioId == null) return default;

        var o = rw.ObservableCache.Lookup(portfolioId) ;
        if (o.HasValue && o.Value.HasValue)
        {
            return o.Value.Value;
        }
        else
        {
            return default;
        }
    }
}


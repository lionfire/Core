using DynamicData;

namespace LionFire.Data.Collections;

public interface IAsyncKeyedCollection<TKey, TItem> : IAsyncCollection<TItem>
    where TKey : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; }

    Task<bool> Remove(TKey key);
}



using DynamicData;

namespace LionFire.Data.Collections;

public interface  IHasObservableCache<TItem, TKey>
    where TKey : notnull
    where TItem : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; }
}


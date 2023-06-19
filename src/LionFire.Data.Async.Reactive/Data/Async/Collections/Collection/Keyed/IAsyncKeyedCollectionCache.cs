﻿using DynamicData;

namespace LionFire.Data.Async.Collections;

public interface IAsyncKeyedCollectionCache<TKey, TItem> : IAsyncCollectionCache<TItem>
    where TKey : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; }

    Task<bool> Remove(TKey key);
}

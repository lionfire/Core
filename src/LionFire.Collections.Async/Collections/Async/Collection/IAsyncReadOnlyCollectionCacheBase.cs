﻿using DynamicData;
using LionFire.Resolves;

namespace LionFire.Collections.Async;

public interface IAsyncReadOnlyCollectionCacheBase<TItem>
    : ILazilyResolves<IEnumerable<TItem>>
    , IObservableResolves<IEnumerable<TItem>>
    , IReadOnlyCollection<TItem>
{
}

public interface IAsyncReadOnlyCollectionCache<TItem>
    //: IReadOnlyCollection<TItem> // Access via List instead
{
    //DynamicData.IObservableList<TItem> List { get; }
}
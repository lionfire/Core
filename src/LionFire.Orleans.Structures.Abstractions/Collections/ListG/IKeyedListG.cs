﻿using DynamicData;
using LionFire.Data.Collections;
using LionFire.Orleans_.ObserverGrains;
using LionFire.Orleans_.Reactive_;

namespace LionFire.Orleans_.Collections;

public interface IKeyedListG<TKey, TItem> 
    : IListBaseG<TItem>
    , IAsyncKeyedCollectionBase<TKey, TItem>
    , IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>>
    where TKey : notnull
{
}

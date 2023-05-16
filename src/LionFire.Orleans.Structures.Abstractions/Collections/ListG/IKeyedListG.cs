using DynamicData;
using LionFire.Collections.Async;
using LionFire.Orleans_.Reactive_;

namespace LionFire.Orleans_.Collections;

public interface IKeyedListG<TKey, TItem> 
    : IListBaseG<TItem>
    , IAsyncKeyedCollectionBase<TKey, TItem>
    , IGrainObservableG<ChangeSet<TItem, TKey>>
    where TKey : notnull
{
}

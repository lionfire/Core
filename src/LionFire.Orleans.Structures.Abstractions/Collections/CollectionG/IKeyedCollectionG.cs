using LionFire.Collections.Async;

namespace LionFire.Orleans_.Collections;

public interface IKeyedCollectionG<TKey, TItem>
    : ICollectionBaseG<TItem>
    , IAsyncKeyedCollectionBase<TKey, TItem>
    , IGrainObservableG<ChangeSet<TItem, TKey>>
    where TKey : notnull
{
}

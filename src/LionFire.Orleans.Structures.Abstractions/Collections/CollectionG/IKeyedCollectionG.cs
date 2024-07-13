using LionFire.Data.Collections;
using LionFire.Orleans_.ObserverGrains;

namespace LionFire.Orleans_.Collections;

public interface IKeyedCollectionG<TKey, TItem>
    : ICollectionBaseG<TItem>
    , IAsyncKeyedCollectionBase<TKey, TItem>
    , IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>>
    where TKey : notnull
    where TItem : notnull
{
    Task<IEnumerable<Type>> SupportedTypes();
}

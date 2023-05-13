using DynamicData;

namespace LionFire.Orleans_.Collections;

public interface IKeyedListG<TKey, TItem>
    : IListBaseG<TItem>
    , IGrainObservableG<IGrainObserverO<ChangeSet<TItem, TKey>>>
    where TKey : notnull
{
    // No Add(TKey, TItem) because this is still a List, not a Dictionary.  The key is always derived from TItem, so the existing Add method is still relevant.

    Task<bool> ContainsKey(TKey key);
    Task<bool> Remove(TKey key);
}

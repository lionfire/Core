namespace LionFire.Data.Collections;

public interface IAsyncKeyedCollectionBase<TKey, TItem> : IAsyncCollectionBase_OLD<TItem>
    where TKey : notnull
{
    Task<bool> ContainsKey(TKey key);
    Task<bool> Remove(TKey key);

    // No Add(TKey, TItem) because this is still a List, not a Dictionary.  The key is always derived from TItem, so the existing Add method is still relevant.
}


namespace LionFire.Collections.Async;

// Features included:
//  - Remove
//  - Resolve
//
// Also consider these:
//  - IAsyncAdds<TItem>
//  - IAsyncCreates<TItem>
//  - IAsyncCreatesForKey<TItem>
public interface IAsyncCollectionCache<TItem> : IAsyncReadOnlyCollectionCache<TItem>
{
    bool IsReadOnly { get; }

    #region Remove

    Task<bool> Remove(TItem item);
    IObservable<(TItem value, Task<bool> result)> Removes { get; }

    #endregion

}

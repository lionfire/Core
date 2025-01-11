namespace LionFire.Data.Collections;

// Features included:
//  - Remove
//  - Get
//
// Also consider these:
//  - IAsyncAdds<TItem>
//  - IAsyncCreates<TItem>
//  - IAsyncCreatesForKey<TItem>
// See also:
//  - IAsyncReadOnlyCollection<TItem>
public interface IAsyncReadOnlyCollection<TItem> : IEnumerableGetter<TItem>
{
    bool IsReadOnly { get; }

    // TODO: Count property?  If so, get it from source asynchronously, or get the local cache's count?  Latter can already be done, so this wouldn't add functionality.
    //Task<int> Count();

}

public interface IAsyncCollection<TItem> : IAsyncReadOnlyCollection<TItem>
{
    #region Remove

    ValueTask<bool> Remove(TItem item);
    IObservable<(TItem item, Task<bool> result)> Removes { get; }

    #endregion

}

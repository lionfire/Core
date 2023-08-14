namespace LionFire.Data.Collections;

// Features included:
//  - Remove
//  - Get
//
// Also consider these:
//  - IAsyncAdds<TItem>
//  - IAsyncCreates<TItem>
//  - IAsyncCreatesForKey<TItem>
public interface IAsyncCollection<TItem> : IEnumerableGetter<TItem>
{
    bool IsReadOnly { get; }

    #region Remove

    Task<bool> Remove(TItem item);
    IObservable<(TItem value, Task<bool> result)> Removes { get; }

    #endregion

}

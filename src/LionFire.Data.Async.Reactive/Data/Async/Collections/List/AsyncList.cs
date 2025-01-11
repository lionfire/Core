namespace LionFire.Data.Collections;

public abstract class AsyncList<TValue> 
    : AsyncReadOnlyList<TValue>
    , IAsyncCollection<TValue>
{
    #region IAsyncCollectionCache<TValue>

    public bool IsReadOnly => isReadOnly;

    private bool isReadOnly = false;
    public void SetIsReadOnly(bool readOnly) => isReadOnly = readOnly;

    #endregion

    #region Remove

    public IObservable<(TValue item, Task<bool> result)> Removes => throw new NotImplementedException();
    public abstract ValueTask<bool> Remove(TValue item);

    #endregion
}
namespace LionFire.Data.Collections;

public abstract class AsyncListCache<TValue> 
    : AsyncReadOnlyListCache<TValue>
    , IAsyncCollectionCache<TValue>
{
    #region IAsyncCollectionCache<TValue>

    public bool IsReadOnly => isReadOnly;

    private bool isReadOnly = false;
    public void SetIsReadOnly(bool readOnly) => isReadOnly = readOnly;

    #endregion

    #region Remove

    public IObservable<(TValue value, Task<bool> result)> Removes => throw new NotImplementedException();
    public abstract Task<bool> Remove(TValue item);

    #endregion
}
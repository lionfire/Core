
using DynamicData;
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

/// <summary>
/// Read-only by default.  Override IsReadOnly and add more interfaces for read-write.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Implementers: override either RetrieveValues or RetrieveImpl</remarks>
public abstract class AsyncKeyValueCollection<TKey, TValue>
    : AsyncReadOnlyKeyValuePairCollection<TKey, TValue>
    , IAsyncKeyValueCollection<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region Lifecycle

    public AsyncKeyValueCollection() : this(null) { }

    public AsyncKeyValueCollection(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null) : base(keySelector, dictionary, options)
    {
    }

    #endregion

    #region IAsyncCollectionCache<TValue>

    #region IsReadOnly

    public bool IsReadOnly => isReadOnly;
    private bool isReadOnly = false;
    public void SetIsReadOnly(bool readOnly) => isReadOnly = readOnly;

    #endregion

    #region Remove

    public abstract ValueTask<bool> Remove(KeyValuePair<TKey, TValue> keyValuePair);
    public abstract ValueTask<bool> Remove(TKey key);
    public virtual ValueTask<bool> Remove(TValue item) => Remove(KeySelector(item));

    public IObservable<(KeyValuePair<TKey, TValue> item, Task<bool> result)> Removes => removes.Value;

    protected Lazy<Subject<(KeyValuePair<TKey, TValue> item, Task<bool> result)>> removes = new Lazy<Subject<(KeyValuePair<TKey, TValue> item, Task<bool> result)>>(LazyThreadSafetyMode.PublicationOnly);

    #endregion

    #region Remove (KVP)

    //public Task<bool> Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Value);
    //IObservable<(KeyValuePair<TKey, TValue> value, Task<bool> result)> IAsyncCollectionCache<KeyValuePair<TKey, TValue>>.Removes => throw new NotImplementedException();

    #endregion

    #endregion


}

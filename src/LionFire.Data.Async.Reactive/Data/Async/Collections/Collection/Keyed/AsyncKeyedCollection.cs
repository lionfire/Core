
using DynamicData;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

/// <summary>
/// Read-only by default.  Override IsReadOnly and add more interfaces for read-write.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Implementors: override either RetrieveValues or RetrieveImpl</remarks>
public abstract class AsyncKeyedCollection<TKey, TValue>
    : AsyncReadOnlyKeyedCollection<TKey, TValue>
    , IAsyncKeyedCollection<TKey, TValue>
    where TKey : notnull
{
    #region Lifecycle

    public AsyncKeyedCollection() : this(null) { }

    public AsyncKeyedCollection(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null) : base(keySelector, dictionary, options)
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

    public virtual Task<bool> Remove(KeyValuePair<TKey, TValue> keyValuePair)
    {
        if (!KeySelector(keyValuePair.Value).Equals(keyValuePair.Key)) throw new ArgumentNullException("Unexpected key for value");
        return Remove(keyValuePair.Key);
    }
    public abstract Task<bool> Remove(TKey key);
    public virtual Task<bool> Remove(TValue item) => Remove(KeySelector(item));

    public IObservable<(TValue item, Task<bool> result)> Removes => removes.Value;
    protected Lazy<Subject<(TValue item, Task<bool> result)>> removes = new Lazy<Subject<(TValue item, Task<bool> result)>>(LazyThreadSafetyMode.PublicationOnly);

    public IObservable<(TKey key, TValue item, Task<bool> result)> RemovesWithKeys => Removes.Select(x => (KeySelector(x.item), x.item, x.result));

    #endregion

    #region Remove (KVP)

    //public Task<bool> Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Value);
    //IObservable<(KeyValuePair<TKey, TValue> value, Task<bool> result)> IAsyncCollectionCache<KeyValuePair<TKey, TValue>>.Removes => throw new NotImplementedException();

    #endregion

    #endregion


}

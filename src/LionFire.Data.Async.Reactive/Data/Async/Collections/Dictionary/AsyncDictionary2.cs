#if FUTURE // maybe
using DynamicData;
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

/// <summary>
/// Read-only by default.  Override IsReadOnly and add more interfaces for read-write.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Implementers: override either RetrieveValues or RetrieveImpl</remarks>
public abstract class AsyncDictionary2<TKey, TValue>
    : AsyncReadOnlyDictionary2<TKey, TValue>
    , IAsyncDictionary2<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region Lifecycle

    public AsyncDictionary2() : this(null) { }

    public AsyncDictionary2(AsyncObservableCollectionOptions? options = null) : base(options)
    {
    }

    #endregion

    #region IAsyncCollectionCache<TValue>

    #region IsReadOnly

    public virtual bool IsReadOnly => false;
    //private bool isReadOnly = false;
    //public void SetIsReadOnly(bool readOnly) => isReadOnly = readOnly;

    #endregion

    #region Mutation

    #region Add

    public abstract ValueTask<bool> TryAdd(TKey key, TValue item);

    #endregion

    #region Upsert

    public abstract ValueTask Upsert(TKey key, TValue item);

    #endregion

    #region Remove

    public abstract ValueTask<bool> Remove(TKey item);

    //public IObservable<(TValue value, Task<bool> result)> Removes => removes.Value;

    //protected Lazy<Subject<(TValue value, Task<bool> result)>> removes = new Lazy<Subject<(TValue value, Task<bool> result)>>(LazyThreadSafetyMode.PublicationOnly);

    #endregion

    //#region Remove (KVP)

    //public ValueTask<bool> Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Value);

    //IObservable<(KeyValuePair<TKey, TValue> item, Task<bool> result)> IAsyncCollection<KeyValuePair<TKey, TValue>>.Removes => throw new NotImplementedException();

    //#endregion

    #endregion


    #endregion
}

#endif
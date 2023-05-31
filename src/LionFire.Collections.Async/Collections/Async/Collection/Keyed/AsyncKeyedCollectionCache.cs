﻿
using DynamicData;
using System.Reactive.Subjects;

namespace LionFire.Collections.Async;

/// <summary>
/// Read-only by default.  Override IsReadOnly and add more interfaces for read-write.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Implementors: override either RetrieveValues or RetrieveImpl</remarks>
public abstract class AsyncKeyedCollectionCache<TKey, TValue>
    : AsyncReadOnlyKeyedCollectionCache<TKey, TValue>
    , IAsyncKeyedCollectionCache<TKey, TValue>
    where TKey : notnull
{
    #region Lifecycle

    public AsyncKeyedCollectionCache() : this(null) { }

    public AsyncKeyedCollectionCache(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null) : base(keySelector, dictionary, options)
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

    public abstract Task<bool> Remove(TKey key);
    public virtual Task<bool> Remove(TValue item) => Remove(KeySelector(item));

    public IObservable<(TValue value, Task<bool> result)> Removes => removes.Value;

    protected Lazy<Subject<(TValue value, Task<bool> result)>> removes = new Lazy<Subject<(TValue value, Task<bool> result)>>(LazyThreadSafetyMode.PublicationOnly);

    #endregion

    #region Remove (KVP)

    //public Task<bool> Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Value);
    //IObservable<(KeyValuePair<TKey, TValue> value, Task<bool> result)> IAsyncCollectionCache<KeyValuePair<TKey, TValue>>.Removes => throw new NotImplementedException();

    #endregion

    #endregion


}

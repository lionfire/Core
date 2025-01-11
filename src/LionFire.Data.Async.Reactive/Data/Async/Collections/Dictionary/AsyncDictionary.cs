
using DynamicData;
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

/// <summary>
/// Read-only by default.  Override IsReadOnly and add more interfaces for read-write.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Implementers: override either RetrieveValues or RetrieveImpl</remarks>
public abstract class AsyncDictionary<TKey, TValue>
    : AsyncReadOnlyDictionary<TKey, TValue>
    , IAsyncDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region Lifecycle

    public AsyncDictionary() : this(null) { }

    public AsyncDictionary(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null) : base(keySelector, dictionary, options)
    {
    }

    #endregion

    #region IAsyncCollectionCache<TValue>

    #region IsReadOnly

    public bool IsReadOnly => isReadOnly;
    private bool isReadOnly = false;
    public void SetIsReadOnly(bool readOnly) => isReadOnly = readOnly;

    #endregion

    #region Mutation

    #region Add

    public abstract ValueTask<bool> TryAdd(TKey key, TValue item);

    #endregion

    #region Upsert

    public abstract ValueTask<bool?> Upsert(TKey key, TValue item);

    #endregion

    #region Remove

    public abstract ValueTask<bool> Remove(TKey item);

    public IObservable<(TValue value, Task<bool> result)> Removes => removes.Value;

    protected Lazy<Subject<(TValue value, Task<bool> result)>> removes = new Lazy<Subject<(TValue value, Task<bool> result)>>(LazyThreadSafetyMode.PublicationOnly);

    #endregion

    #region Remove (KVP)

    public ValueTask<bool> Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Value);

    IObservable<(KeyValuePair<TKey, TValue> item, Task<bool> result)> IAsyncCollection<KeyValuePair<TKey, TValue>>.Removes => throw new NotImplementedException();

    #endregion

    #endregion


    #endregion
}

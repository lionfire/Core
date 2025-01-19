using DynamicData;
using LionFire.Data.Async;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;



//public struct Label<TKey, TValue>
//    where TKey : notnull
//    where TValue : notnull
//{
//    public TKey Key;
//    public TValue Value;
//}
#if false //OLD: This is what AsyncDictionary is for
public abstract class AsyncLabelledCollection<TKey, TValue> : AsyncKeyedCollection<TKey, Label<TKey, TValue>>
{
    protected AsyncLabelledCollection(SourceCache<Label<TKey, TValue>, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null, IObservable<IChangeSet<Label<TKey, TValue>, TKey>?>? keyValueChanges = null) : base(v => v.Key, dictionary, options, keyValueChanges)
    {
    }
}
#endif

/// <summary>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <remarks>Implementers: override either RetrieveValues or RetrieveImpl</remarks>
public abstract class AsyncKeyedCollection<TKey, TValue>
    : AsyncReadOnlyKeyedCollection<TKey, TValue>
    , IAsyncKeyedCollection<TKey, TValue>
    //, IObservableSetState // Consider adding this here and/or in concrete class
    where TKey : notnull
    where TValue : notnull
{
    #region Lifecycle

    public AsyncKeyedCollection() : this(null) { }

    public AsyncKeyedCollection(Func<TValue, TKey>? keySelector, SourceCache<TValue, TKey>? dictionary = null, AsyncObservableCollectionOptions? options = null, IObservable<IChangeSet<TValue, TKey>?>? keyValueChanges = null) : base(keySelector, dictionary, options, keyValueChanges)
    {
    }

    #endregion

    #region IAsyncCollectionCache<TValue>

    #region IsReadOnly

    public virtual bool IsReadOnly => false;
    //public void SetIsReadOnly(bool readOnly) => isReadOnly = readOnly;

    #endregion

    #region Add

    public abstract ValueTask Add(TValue value);
    public abstract ValueTask Upsert(TValue value);

    #endregion

    #region Remove

    public virtual ValueTask<bool> Remove(KeyValuePair<TKey, TValue> keyValuePair)
    {
        if (!KeySelector(keyValuePair.Value).Equals(keyValuePair.Key)) throw new ArgumentNullException("Unexpected key for value");
        return Remove(keyValuePair.Key);
    }
    public abstract ValueTask<bool> Remove(TKey key);
    public virtual ValueTask<bool> Remove(TValue item) => Remove(KeySelector(item));

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

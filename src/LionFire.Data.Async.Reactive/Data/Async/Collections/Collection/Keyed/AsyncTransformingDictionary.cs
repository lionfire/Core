#if DISCARD
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

public abstract class AsyncTransformingDictionary<TKey, TUnderlying, TUsable>
    : AsyncTransformingReadOnlyDictionary<TKey, TUnderlying, TUsable>
    , IAsyncDictionary<TKey, TUsable>
    //, IObservableSetState // Consider adding this here, but noop implementation if underlying doesn't support
    where TKey : notnull
    where TUnderlying : notnull
    where TUsable : notnull
{

    #region Lifecycle

    protected AsyncTransformingDictionary(AsyncKeyedCollection<TKey, TUnderlying> underlying) : base(underlying)
    {
    }

    #endregion

    public override bool IsReadOnly => false;

    //IEnumerable<KeyValuePair<TKey, TUsable>>? IGetter<IEnumerable<KeyValuePair<TKey, TUsable>>>.ReadCacheValue => throw new NotImplementedException();

    //IEnumerable<KeyValuePair<TKey, TUsable>>? IReadWrapper<IEnumerable<KeyValuePair<TKey, TUsable>>>.Value => throw new NotImplementedException();

    //IObservable<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TUsable>>>>> IObservableGetOperations<IEnumerable<KeyValuePair<TKey, TUsable>>>.GetOperations => throw new NotImplementedException();

    //IObservable<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TUsable>>>>> IObservableGetOperations<IEnumerable<KeyValuePair<TKey, TUsable>>>.GetOperations => getOperations;



    #region Mutation

    public virtual ValueTask<bool> TryAdd(TKey key, TUsable item) => underlying.TryAdd(key, Serialize(item));
    public virtual ValueTask Upsert(TKey key, TUsable item) => underlying.Upsert(key, Serialize(item));

    //public virtual ValueTask<bool> Remove(TUsable item) => throw new NotSupportedException();

    public virtual ValueTask<bool> Remove(TKey key) => underlying.Remove(key);

    //public override Task<bool> Remove(TUsable item) => throw new NotSupportedException(); // Remove(KeySelector(item));

    #endregion

}

#endif
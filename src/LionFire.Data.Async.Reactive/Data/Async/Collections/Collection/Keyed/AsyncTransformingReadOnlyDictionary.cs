#if false // DISCARD
using DynamicData;
using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

public abstract class AsyncTransformingReadOnlyDictionary<TKey, TUnderlying, TUsable>
    : GetterRxO<KeyValuePair<TKey, TUsable>>
    , IAsyncReadOnlyDictionary<TKey, TUsable>
    //, IObservableGetState // Consider adding this here, but noop implementation if underlying doesn't support
where TKey : notnull
where TUnderlying : notnull
where TUsable : notnull
{

    protected override ITask<IGetResult<KeyValuePair<TKey, TUsable>>> GetImpl(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #region Lifecycle

    public AsyncTransformingReadOnlyDictionary(AsyncDictionary2<TKey, TUnderlying> underlying)
    {
        this.underlying = underlying;

        lazyUsableObservableCache = new(() => underlying.ObservableCache.Connect().Transform(underlying
            => Deserialize(underlying)).RefCount().AsObservableCache());
    }

    #endregion

    #region Underlying

    public AsyncDictionary<TKey, TUnderlying> Underlying => underlying;
    protected AsyncDictionary<TKey, TUnderlying> underlying;

    #endregion

    #region Usable

    public IObservableCache<(TKey key, TUsable value), TKey> ObservableCache => lazyUsableObservableCache.Value;
    private Lazy<IObservableCache<(TKey key, TUsable value), TKey>> lazyUsableObservableCache;

    #endregion

    #region IAsyncKeyedCollection

    public virtual bool IsReadOnly => true;

    #region Value

    public IEnumerable<TUsable>? ReadCacheValue => throw new NotImplementedException();

    public IEnumerable<TUsable>? Value => throw new NotImplementedException();

    public bool HasValue => throw new NotImplementedException();

    #endregion

    #region Events

    public IObservable<ITask<IGetResult<IEnumerable<TUsable>>>> GetOperations => getOperations;
    protected Subject<ITask<IGetResult<IEnumerable<KeyValuePair<TKey, TUsable>>>>> getOperations = new();

    public virtual IObservable<(TUsable item, Task<bool> result)> Removes => throw new NotImplementedException();

    #endregion


    #region Retrieve

    // TODO: Auto-get upon Subscribe

    public async ITask<IGetResult<IEnumerable<TUsable>>> GetIfNeeded() => GetResult<IEnumerable<TUsable>>.TransformFrom(await underlying.GetIfNeeded(), e => e.Select(Deserialize));

    public IGetResult<IEnumerable<TUsable>> QueryGetResult()
        => GetResult<IEnumerable<TUsable>>.TransformFrom(underlying.QueryGetResult(), e => e.Select(Deserialize));

    public async ITask<IGetResult<IEnumerable<TUsable>>> Get(CancellationToken cancellationToken = default)
        => GetResult<IEnumerable<TUsable>>.TransformFrom(await underlying.Get(cancellationToken), e => e.Select(Deserialize));

    #endregion

    #endregion

    #region IDiscardable / IDiscardableValue

    // TODO: Eliminate IDiscardableValue and IDiscardable, and add back in if/where appropriate
    public void DiscardValue() => underlying.DiscardValue();
    public void Discard() => underlying.Discard();

    #endregion

    #region IEnumerable

    //public IEnumerator<TUsable> GetEnumerator() => ObservableCache.Items.GetEnumerator();
    //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Serialization (Transformation)

    protected abstract TUnderlying Serialize(TUsable usable);
    protected abstract TUsable Deserialize(TUnderlying underlying);

    #endregion
}


#endif
using LionFire.Data.Async.Sets;

namespace LionFire.Data.Async;

public interface IAwareOfSets<TValue>
{
    void OnSet(ISetResult<TValue> result);
}

public interface IAsyncValue<TValue>
    : IValueRxO<TValue>
    , IAwareOfSets<TValue>
{
    new TValue Value { get; set; }

    /// <summary>
    /// Guaranteed not to block, even if BlockOnGet is enabled
    /// </summary>
    TValue? QueryValue { get; }
}

#if false // TODO: Needs non-abstract AsyncGets

public abstract class AsyncCompositeValue<TValue>
    : ReactiveObject
    , ILazilyGetsRx<TValue>
//, IAsyncValueRx<TValue>
{
    #region Parameters

    public ValueOptions Options { get; }

    #endregion

    #region Components

    public AsyncGets<TValue> Gets { get; }
    public AsyncSets<TValue> Sets { get; }

    #endregion

    #region Lifecycle

    public AsyncCompositeValue() : this(null) { }
    public AsyncCompositeValue(ValueOptions? options)
    {
        Options = options ?? ValueOptions<TValue>.Default;
        Gets = new(Options.Get);
        Sets = new(Options.Set);
    }

    #endregion

    #region Gets pass-thru

    public TValue? ReadCacheValue => ((ILazilyGets<TValue>)Gets).ReadCacheValue;

    public TValue? Value => ((IReadWrapper<TValue>)Gets).Value;

    public bool HasValue => ((IDefaultable)Gets).HasValue;

    public AsyncGetOptions Object { get => ((IHasNonNullSettable<AsyncGetOptions>)Gets).Object; set => ((IHasNonNullSettable<AsyncGetOptions>)Gets).Object = value; }

    AsyncGetOptions IHasNonNull<AsyncGetOptions>.Object => ((IHasNonNull<AsyncGetOptions>)Gets).Object;

    public ITask<IGetResult<TValue>> GetIfNeeded()
    {
        return ((ILazilyGets<TValue>)Gets).GetIfNeeded();
    }

    public IGetResult<TValue> QueryValue()
    {
        return ((ILazilyGets<TValue>)Gets).QueryValue();
    }

    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        return ((IGets<TValue>)Gets).Get(cancellationToken);
    }

    public void DiscardValue()
    {
        ((IDiscardableValue)Gets).DiscardValue();
    }

    public void Discard()
    {
        ((IDiscardable)Gets).Discard();
    }

    #endregion

}
#endif
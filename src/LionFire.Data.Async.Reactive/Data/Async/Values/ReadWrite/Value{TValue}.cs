using LionFire.Data.Reactive;
using LionFire.Data.Async.Sets;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace LionFire.Data.Async;

public abstract class Value<TValue>
    : GetterRxO<TValue>
    , IValueRx<TValue>
    , ISetterRxO<TValue>
    , ISetsInternal<TValue>
    , IValue<TValue>
{
    #region Options

    #region (static)

    public new static AsyncValueOptions DefaultOptions => AsyncValueOptions<TValue>.Default;

    #endregion

    public AsyncValueOptions Options { get; set; }
    //AsyncValueOptions IHasNonNullSettable<AsyncValueOptions>.Object { get => Options; set => Options = value; }

    //AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    #endregion

    #region Lifecycle

    public Value() : base(DefaultOptions.Get)
    {
        Options = DefaultOptions;
    }

    public Value(AsyncValueOptions options) : base(options.Get)
    {
        Options = options;

        this.ObservableForProperty(t => t.ReadCacheValue)
            .Subscribe(t =>
            {
                if (HasStagedValue)
                {
                    ValueChangedWhileValueStaged = true;
                }
            });
    }

    #endregion

    #region Get+Set 

    [Reactive]
    public bool ValueChangedWhileValueStaged { get; set; }

    #endregion

    #region Get (override)

    public override ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var setState = SetState;
        if (SetsLogic<TValue>.IsSetStateSetting(setState) && Options.OptimisticGetWhileSetting)
        {
            // return Optimistically
            return Task.FromResult<IGetResult<TValue>>(new OptimisticGetResult<TValue>(setState.DesiredValue)).AsITask();
        }
        return base.Get(cancellationToken);
    }

    #endregion

    #region Set

    object ISetsInternal<TValue>.setLock { get; } = new();

    #region State

    [Reactive]
    public TValue? StagedValue { get; set; }

    [Reactive]
    public bool HasStagedValue { get; set; }

    public void DiscardStagedValue()
    {
        using var _ = DelayChangeNotifications();
        HasStagedValue = false;
        StagedValue = default;
        ValueChangedWhileValueStaged = false;
    }

    #endregion

    #region Status

    public ISetOperation<TValue> SetState => sets.Value;

    #endregion

    #region Events

    [Browsable(false)]
    public IObservable<ISetOperation<TValue>> SetOperations => sets;
    private BehaviorSubject<ISetOperation<TValue>> sets = SetsLogic<TValue>.InitSets;
    BehaviorSubject<ISetOperation<TValue>> ISetsInternal<TValue>.sets => sets;

    #endregion


    #region IGetsOrAsyncInstantiates<T>
    //public abstract ITask<TValue> InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
    //public abstract ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();
    #endregion

    #region Methods
    public abstract Task<ITransferResult> SetImpl(TValue? value, CancellationToken cancellationToken = default);

    public Task<ITransferResult> Set(CancellationToken cancellationToken = default)
        => SetsLogic<TValue>.Set(this, cancellationToken);

    public Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default)
        => SetsLogic<TValue>.Set(this, value, cancellationToken);

    #endregion

    #endregion

}

#if false // TODO: Needs non-abstract AsyncGets

public abstract class AsyncCompositeValue<TValue>
    : ReactiveObject
    , ILazilyGetsRx<TValue>
//, IAsyncValueRx<TValue>
{
    #region Parameters

    public AsyncValueOptions Options { get; }

    #endregion

    #region Components

    public AsyncGets<TValue> Gets { get; }
    public AsyncSets<TValue> Sets { get; }

    #endregion

    #region Lifecycle

    public AsyncCompositeValue() : this(null) { }
    public AsyncCompositeValue(AsyncValueOptions? options)
    {
        Options = options ?? AsyncValueOptions<TValue>.Default;
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
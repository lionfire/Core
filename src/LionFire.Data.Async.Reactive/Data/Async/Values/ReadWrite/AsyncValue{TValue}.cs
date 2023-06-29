using LionFire.Data.Reactive;
using LionFire.Data.Sets;
using LionFire.Results;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace LionFire.Data;

public abstract class AsyncValue<TValue>
    : AsyncGets<TValue>
    , IAsyncValueRx<TValue>
{
    #region Options

    #region (static)

    public new static AsyncValueOptions DefaultOptions => AsyncValueOptions<TKey, TValue>.Default;

    #endregion

    public new AsyncValueOptions Options
    {
        get => (AsyncValueOptions)base.Options;
        set => base.Options = value;
    }
    AsyncValueOptions IHasNonNullSettable<AsyncValueOptions>.Object { get => Options; set => Options = value; }

    AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    #endregion

    #region Lifecycle

    public AsyncValue() : base(DefaultOptions)
    {
        asyncSets = new();
    }

    public AsyncValue(AsyncValueOptions options) : base(options)
    {
        asyncSets = new(options);

        this.ObservableForProperty(t => t.Value)
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
        if (IsSetStateSetting(setState) && Options.OptimisticGetWhileSetting)
        {
            // return Optimistically
            return Task.FromResult<IGetResult<TValue>>(new OptimisticGetResult<TValue>(setState.SettingToValue)).AsITask();
        }
        return base.Get(cancellationToken);
    }

    #endregion

    AsyncSets<TValue> asyncSets;
    #region Set

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

    #endregion

    #region Events

    [Browsable(false)]
    public IObservable<ITask<ITransferResult>> Sets => sets ??= new();
    private Subject<ITask<ITransferResult>>? sets;

    #endregion

    #region Methods

    public abstract Task<ITransferResult> SetImpl(CancellationToken cancellationToken = default);

    public async Task<ITransferResult> Set(CancellationToken cancellationToken = default)
    {
        var result = await SetImpl(StagedValue, cancellationToken);
        if (result.IsSuccess())
        {
            HasStagedValue = false;
        }
    }

    public Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    #endregion

}

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

    public ITask<ILazyGetResult<TValue>> GetIfNeeded()
    {
        return ((ILazilyGets<TValue>)Gets).GetIfNeeded();
    }

    public ILazyGetResult<TValue> QueryValue()
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
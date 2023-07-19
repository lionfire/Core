using LionFire.Data.Reactive;
using LionFire.Data.Sets;
using LionFire.Results;
using MorseCode.ITask;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace LionFire.Data;


internal interface IGetsInternal<TValue> //: IAsyncGetsRx<TValue>
{
    IEqualityComparer<TValue> EqualityComparer { get; }
}

internal interface ISetsInternal<TValue> : IAsyncValueRx<TValue>
{
    IEqualityComparer<TValue> EqualityComparer { get; }
    ISetOperation<TValue> SetState { get; }
    object setLock { get; }
    BehaviorSubject<ISetOperation<TValue>> sets { get; }
    Task<ITransferResult> SetImpl(TValue? value, CancellationToken cancellationToken = default);
}

public abstract class AsyncValue<TKey, TValue>
    : Gets<TKey, TValue>
    , IAsyncValueRx<TValue>
    , ISetsInternal<TValue>
{
    #region Parameters


    #region Options

    #region (static)

    public static new AsyncValueOptions DefaultOptions => AsyncValueOptions<TKey, TValue>.Default;

    //public static IEqualityComparer<TValue> DefaultEqualityComparer => EqualityComparerOptions<TValue>.Default;

    #endregion

    public AsyncValueOptions Options
    {
        get => options;
        set
        {
            options = value;
            base.GetOptions = value.Get; // redundant reference to GetOptions.
        }
    }
    AsyncValueOptions options;

    //AsyncValueOptions IHasNonNullSettable<AsyncValueOptions>.Object { get => Options; set => Options = value; }

    //AsyncValueOptions IHasNonNull<AsyncValueOptions>.Object => Options;

    #endregion


    #endregion

    #region Lifecycle

    public AsyncValue(TKey key, AsyncValueOptions? options = null) : base(key, options?.Get ?? DefaultOptions.Get)
    {
        Options = options ?? AsyncObjectKey?.Options.ValueOptions ?? DefaultOptions;

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

    #region Get (override)

    public override ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var setState = SetState;
        if (AsyncSetLogic<TValue>.IsSetStateSetting(setState) && Options.OptimisticGetWhileSetting)
        {
            // return Optimistically
            return Task.FromResult<IGetResult<TValue>>(new OptimisticGetResult<TValue>(setState.DesiredValue)).AsITask();
        }
        return base.Get(cancellationToken);
    }

    #endregion

    #region Get+Set 

    [Reactive]
    public bool ValueChangedWhileValueStaged { get; set; }

    #endregion

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

    public bool IsSetting => AsyncSetLogic<TValue>.IsSetStateSetting(SetState);

    /// <remarks>
    /// locked by: setLock
    /// </remarks>
    public ISetOperation<TValue> SetState => sets.Value;

    #endregion

    #region Events

    public IObservable<ISetOperation<TValue>> Sets => sets;
    private BehaviorSubject<ISetOperation<TValue>> sets = AsyncSetLogic<TValue>.InitSets;
    BehaviorSubject<ISetOperation<TValue>> ISetsInternal<TValue>.sets => sets;

    #endregion

    #region Methods

    public Task<ITransferResult> Set(CancellationToken cancellationToken = default)
        => AsyncSetLogic<TValue>.Set(this, cancellationToken);

    public Task<ITransferResult> SetImpl(TValue? value, CancellationToken cancellationToken = default)
        => SetImpl(Key, value, cancellationToken);

    public abstract Task<ITransferResult> SetImpl(TKey key, TValue? value, CancellationToken cancellationToken = default);

    object ISetsInternal<TValue>.setLock { get; } = new();

    /// <summary>
    /// Skips set if DefaultEqualityComparer.Equals(currentSetState.Value.SettingToValue, value)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ITransferResult> Set(TValue? value, CancellationToken cancellationToken = default)
    {
        var target = Key;
        ArgumentNullException.ThrowIfNull(target, nameof(Key));

        return AsyncSetLogic<TValue>.Set(this, value, cancellationToken);
    }

    #endregion

    #endregion
}

using LionFire.Data.Async.Sets;
using System.Reactive.Subjects;

namespace LionFire.Data.Async;

public abstract class Value<TKey, TValue>
    : Getter<TKey, TValue>
    , IValueRx<TValue>
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

    public Value(TKey key, AsyncValueOptions? options = null) : base(key, options?.Get ?? DefaultOptions.Get)
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
        if (SetsLogic<TValue>.IsSetStateSetting(setState) && Options.OptimisticGetWhileSetting)
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

    public bool IsSetting => SetsLogic<TValue>.IsSetStateSetting(SetState);

    /// <remarks>
    /// locked by: setLock
    /// </remarks>
    public ISetOperation<TValue> SetState => sets.Value;

    #endregion

    #region Events

    public IObservable<ISetOperation<TValue>> SetOperations => sets;
    private BehaviorSubject<ISetOperation<TValue>> sets = SetsLogic<TValue>.InitSets;
    BehaviorSubject<ISetOperation<TValue>> ISetsInternal<TValue>.sets => sets;

    #endregion

    #region Methods

    public Task<ITransferResult> Set(CancellationToken cancellationToken = default)
        => SetsLogic<TValue>.Set(this, cancellationToken);

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

        return SetsLogic<TValue>.Set(this, value, cancellationToken);
    }

    #endregion

    #endregion

    #region IGetsOrAsyncInstantiates<T>

    //public abstract ITask<TValue> InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
    //public abstract ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();

    #endregion
}

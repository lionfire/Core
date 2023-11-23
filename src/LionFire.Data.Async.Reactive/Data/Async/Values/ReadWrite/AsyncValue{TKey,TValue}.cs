using LionFire.Data.Async.Sets;
using System.Reactive.Subjects;

namespace LionFire.Data.Async;

public abstract class AsyncValue<TKey, TValue>
    : Getter<TKey, TValue>
    , IValueRxO<TValue>
    , ISetsInternal<TValue>
{
    #region Parameters


    #region Options

    #region (static)

    public static new ValueOptions DefaultOptions => ValueOptions<TKey, TValue>.Default;

    //public static IEqualityComparer<TValue> DefaultEqualityComparer => EqualityComparerOptions<TValue>.Default;

    #endregion

    public ValueOptions Options
    {
        get => options;
        set
        {
            options = value;
            base.GetOptions = value.Get; // redundant reference to GetOptions.
        }
    }
    ValueOptions options;

    //ValueOptions IHasNonNullSettable<ValueOptions>.Object { get => Options; set => Options = value; }
    //ValueOptions IHasNonNull<ValueOptions>.Object { get => Options; }

    //SetterOptions IHasNonNullSettable<SetterOptions>.Object { get => Options.Set; set => Options.Set = value; }
    //SetterOptions IHasNonNull<SetterOptions>.Object { get => Options.Set; }

    #endregion


    #endregion

    #region Lifecycle

    public AsyncValue(TKey key, ValueOptions? options = null) : base(key, options?.Get ?? DefaultOptions.Get)
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

        this.WhenAnyValue(x => x.ReadCacheValue, x => x.StagedValue)
            .Subscribe(t =>
            {
                this.RaisePropertyChanged(nameof(Value));
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

    public new TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get => HasStagedValue
                ? StagedValue
                : (!HasValue && GetOptions.BlockToGet)
                    ? GetIfNeeded().Result.Value // BLOCKING
                    : ReadCacheValue;
        set => StagedValue = value;
    }

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
    public ISetOperation<TValue> SetState => setOperations.Value;

    #endregion

    #region Events

    public IObservable<ISetOperation<TValue>> SetOperations => setOperations;
    private BehaviorSubject<ISetOperation<TValue>> setOperations = new(NoopSetOperation<TValue>.Instantiated);
    BehaviorSubject<ISetOperation<TValue>> ISetsInternal<TValue>.sets => setOperations;

    public IObservable<ISetResult<TValue>> SetResults => setResults;
    private BehaviorSubject<ISetResult<TValue>> setResults = new(NoopSetResult<TValue>.Instantiated);

    #endregion

    #region Methods

    public abstract Task<ISetResult<TValue>> SetImpl(TValue? value, CancellationToken cancellationToken = default);
    //=> SetImpl(value, cancellationToken).AsITask();

    //public abstract Task<ISetResult<TValue>> SetImpl(TValue? value, CancellationToken cancellationToken = default);
    //public abstract Task<ISetResult<T>> SetImpl<T>(TKey key, T? value, CancellationToken cancellationToken = default) where T : TValue;


    object ISetsInternal<TValue>.setLock { get; } = new();

    public async Task<ISetResult> Set(CancellationToken cancellationToken = default)
        => await SetsLogic<TValue>.Set(this, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Skips set if DefaultEqualityComparer.Equals(currentSetState.Value.SettingToValue, value)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ISetResult<TValue>> Set(TValue? value, CancellationToken cancellationToken = default)
    {
        var target = Key;
        ArgumentNullException.ThrowIfNull(target, nameof(Key));

        return SetsLogic<TValue>.Set(this, value, cancellationToken);
    }
    //public ITask<ISetResult<T>> Set<T>(T? value, CancellationToken cancellationToken = default) where T : TValue

    #endregion

    #endregion

    #region IGetsOrAsyncInstantiates<T>

    //public abstract ITask<TValue> InstantiateValue(bool overwriteStagedValue = false, bool throwOnOverwrite = false);
    //public abstract ITask<IGetResult<TValue>> GetOrAsyncInstantiateValue();

    #endregion
}

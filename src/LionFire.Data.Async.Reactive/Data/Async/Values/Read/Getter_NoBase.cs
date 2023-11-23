
namespace LionFire.Data.Async;

#if UNUSED // TRIAGE threadsafety logic to GetterRxO
public abstract class Getter_NoBase<TKey, TValue> // TODO: Base this class on AsyncGets<TValue>?
    : ReactiveObject
    , ILazilyGetsRx<TValue>
    , IDisposable
    , IKeyed<TKey>
{
    #region Parameters

    #region (static)

    public static AsyncGetOptions DefaultOptions
    {
        get => AsyncGetOptions<TKey, TValue>.Default;
        set => AsyncGetOptions<TKey, TValue>.Default = value;
    }

    #endregion

    #region Key

    public TKey Key => key ?? throw new ObjectDisposedException(null);
    protected TKey? key;

    public IAsyncObject? AsyncObjectKey => Key as IAsyncObject;

    #endregion

    #region Options

    public AsyncGetOptions GetOptions { get; set; } = AsyncGetOptions<TKey, TValue>.Default;

    AsyncGetOptions IHasNonNullSettable<AsyncGetOptions>.Object { get => GetOptions; set => GetOptions = value; }
    //AsyncGetOptions IHasNonNull<AsyncGetOptions>.Object => GetOptions;

    #endregion

    #endregion

    #region Lifecycle

    protected AsyncGets_NoBase(TKey key, AsyncGetOptions? options = null)
    {
        this.key = key;
        GetOptions = options ?? DefaultOptions;
    }

    public virtual void Dispose()
    {
        // TODO: THREADSAFETY
        isDisposed = true;
        key = default;
    }
    protected bool isDisposed;

    #endregion

    #region Value

    [Reactive]
    public TValue? ReadCacheValue { get; private set; }

    public TValue? Value
    {
        
        //[Blocking(Alternative = nameof(GetIfNeeded), $"Blocks if !HasValue && {nameof(GetOptions)}.{nameof(GetOptions.GetOnDemand)} && {nameof(GetOptions)}.{nameof(GetOptions.BlockToGet)}")] // TODO
        //get
        //{
        //    return ReadCacheValue ?? (DefaultOptions.BlockToGet && DefaultOptions.GetOnDemand ? GetIfNeeded().Result.Value : default);
        //}
        get
        {
            if (!HasValue)
            {
                if (GetOptions.GetOnDemand)
                {
                    var getTask = Get();
                    if (GetOptions.BlockToGet)
                    {
                        Debugger.NotifyOfCrossThreadDependency();

                        getTask.GetAwaiter().GetResult(); // BLOCKING
                    }
                    //else // OLD
                    //{
                    //    getTask.FireAndForget();
                    //}
                }
                else if (GetOptions.ThrowOnGetValueIfHasValueIsFalse) { DoThrowOnGetValueIfHasValueIsFalse(); }
            }
            return ReadCacheValue;
        }
    }

    private void DoThrowOnGetValueIfHasValueIsFalse() => throw new Exception($"{nameof(Value)} has not been gotten yet.  Invoke Get first or disable {nameof(GetOptions)}.{nameof(GetOptions.ThrowOnGetValueIfHasValueIsFalse)}");

    [Reactive]
    public bool HasValue { get; private set; }
    public IGetResult<TValue> QueryValue() => new LazyResolveResult<TValue>(HasValue, Value);

    public void Discard() => DiscardValue();
    public void DiscardValue()
    {
        Value = default;
        HasValue = false;
    }

    #endregion

    #region Get
        
    #region Status

    public bool IsGetting => GetState.AsTask().IsCompleted == false;

    public ITask<IGetResult<TValue>>? GetState => gets.Value;

    #endregion

    #region Methods

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    public virtual async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default);
        // Moved to GetterRxO

    public async ITask<IGetResult<TValue>> GetIfNeeded()
    {
        if (HasValue) { return QueryValue(); }
        var result = await Get().ConfigureAwait(false);
        return new LazyResolveResult<TValue>(result.IsSuccess == true, result.Value);
    }

    #endregion

    #region Events
        
    public IObservable<ITask<IGetResult<TValue>>> Gets => gets;
    private BehaviorSubject<ITask<IGetResult<TValue>>> gets = new(Task.FromResult<IGetResult<TValue>>(ResolveResultNotResolvedNoop<TValue>.Instance).AsITask());
    
    #endregion

    #endregion
}

#endif
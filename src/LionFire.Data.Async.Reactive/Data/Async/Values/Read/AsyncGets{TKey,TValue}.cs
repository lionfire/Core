

namespace LionFire.Data;

public abstract class AsyncGets<TKey, TValue>
    : AsyncGets<TValue>
    , IDisposable
    , IKeyed<TKey>
{
    #region Key

    public TKey Key => isDisposed ? throw new ObjectDisposedException(null) : key;
    protected TKey key;

    public IAsyncObject? AsyncObjectKey => Key as IAsyncObject;

    #endregion

    #region Lifecycle

    //protected AsyncGets() : this(default, null)
    //{
    //}

    //protected AsyncGets(AsyncGetOptions? options) : base(options)
    //{
    //}
    protected AsyncGets(TKey key, AsyncGetOptions? options) : base(options)
    {
        this.key = key;
    }

    public virtual void Dispose()
    {
        isDisposed = true;
        var keyCopy = key;
        key = default;
        if (keyCopy is IDisposable d && GetOptions.DisposeKey) // ENH: Offload this to implementors
        {
            d.Dispose();
        }
    }
    protected bool isDisposed;

    #endregion
}


#if UNUSED // TRIAGE threadsafety logic
/// <summary>
/// Inheritors must implement GetImpl
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class AsyncGets_NoBase<TKey, TValue> // TODO: Base this class on AsyncGets<TValue>?
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
    public ILazyGetResult<TValue> QueryValue() => new LazyResolveResult<TValue>(HasValue, Value);

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

    SemaphoreSlim getSemaphore = new(1, 1);
    public virtual async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        // TODO: Semaphore, TOTHREADSAFETY - (done in <TValue> version?)

        async Task<IGetResult<TValue>>? ReturnResultInProgress()
        {
            var getTask = GetState;
            if (getTask != null && !getTask.AsTask().IsCompleted) { return await getTask.ConfigureAwait(false); }
            return null;
        }

        Task<IGetResult<TValue>>? task = ReturnResultInProgress();
        if (task != null) return await task.ConfigureAwait(false);

        try
        {
            await getSemaphore.WaitAsync(cancellationToken);

            task = ReturnResultInProgress();
            if (task != null) return await task.ConfigureAwait(false);
            else
            {
                var iTask = GetImpl(cancellationToken);
                task = iTask.AsTask();
                gets.OnNext(iTask);
            }
        }
        finally
        {
            getSemaphore.Release();
        }

        var result = await task.ConfigureAwait(false);
        Value = result.IsSuccess == true ? result.Value : default;
        HasValue = result.IsSuccess == true;
        return result;
    }

    public async ITask<ILazyGetResult<TValue>> GetIfNeeded()
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
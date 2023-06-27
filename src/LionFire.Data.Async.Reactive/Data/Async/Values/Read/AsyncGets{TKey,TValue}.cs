
using DynamicData;
using Microsoft.Extensions.Options;

namespace LionFire.Data.Async;

/// <summary>
/// Inheritors must implement GetImpl
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class AsyncGets<TKey, TValue>
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

    //[SetOnce]
    public TKey Key => key;
    //{
    //    get => isDisposed ? throw new ObjectDisposedException(nameof(DisposableKeyed<TKey>)) : key;
    //    set
    //    {
    //        //if (ReferenceEquals(key, value)) return;
    //        if (EqualityComparer<TKey>.Default.Equals(key, value)) return;
    //        if (!EqualityComparer<TKey>.Default.Equals(key, default)) throw new AlreadySetException();
    ////if (key != default) throw new AlreadySetException();
    //key = value;
    //    }
    //}
    protected TKey key;

    public virtual void Dispose()
    {
        // TODO: THREADSAFETY
        isDisposed = true;
        key = default;
    }
    protected bool isDisposed;

    #endregion

    #region Options

    public AsyncGetOptions GetOptions { get; set; } = AsyncGetOptions<TKey, TValue>.Default;

    AsyncGetOptions IHasNonNullSettable<AsyncGetOptions>.Object { get => GetOptions; set => GetOptions = value; }

    #endregion

    #endregion

    #region Lifecycle

    protected AsyncGets(TKey key, AsyncGetOptions? options = null)
    {
        this.key = key;
        GetOptions = options ?? DefaultOptions;
    }

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

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;
    public ITask<IGetResult<TValue>>? GetState => getState;

    AsyncGetOptions IHasNonNull<AsyncGetOptions>.Object => throw new NotImplementedException();
    private ITask<IGetResult<TValue>>? getState;

    public virtual async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var getTask = GetState;
        if (getTask != null && getState.AsTask().IsCompleted) { return await getTask.ConfigureAwait(false); }

        var result = await GetImpl(cancellationToken).ConfigureAwait(false);
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
}


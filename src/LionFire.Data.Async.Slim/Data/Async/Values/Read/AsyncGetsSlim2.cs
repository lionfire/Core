using MorseCode.ITask;
using System.Threading.Tasks;

namespace LionFire.Data.Gets;

// TODO: Dupe of GetsSlim? What's the difference?

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// AsyncGets&lt;T&gt; also implements ILazilyGets&lt;T&gt; and provides Rx features. Consider using it.
/// </remarks>
public abstract class AsyncGetsSlim2<TValue> : IGets<TValue>
{
    #region Configuration

    public static bool DisposeValue => AsyncGetOptions<TValue>.Default.DisposeValue;

    #endregion

    #region Value

    public bool HasValue { get; protected set; } // 

    public TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get => Value ?? GetIfNeeded().Result.Value;
    }

    public TValue? ReadCacheValue
    {
        get => readCacheValue;
        protected set
        {
            if (EqualityComparer<TValue>.Default.Equals(readCacheValue, value)) return;
            var oldValue = readCacheValue;

            // TODO: Move this to a finally block?
            if (DisposeValue && oldValue is IDisposable d)
            {
                Log.Get<AsyncGetsSlim2<TValue>>().LogDebug("Disposing object of type {Type}", d.GetType().FullName);
                d.Dispose();
            }

            readCacheValue = value;

            OnValueChanged(value, oldValue);
        }
    }
    protected TValue? readCacheValue;

    /// <summary>
    /// Raised when ReadCacheValue changes
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    protected virtual void OnValueChanged(TValue? newValue, TValue? oldValue)
    {
    }

    #region Discard

    public virtual void Discard() => DiscardValue();
    public void DiscardValue()
    {
        ReadCacheValue = default;
        
        HasValue = false;
    }

    #endregion

    #endregion

    #region GetValue

    private SemaphoreSlim TryGetSemaphore = new SemaphoreSlim(1);

    public async ITask<ILazyGetResult<TValue>> GetIfNeeded()
    {
        await TryGetSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            var lastGetResult = QueryValue();
            if (lastGetResult.HasValue) return lastGetResult;
            if (HasValue) { return QueryValue(); } // TODO: 
            var result = await Get().ConfigureAwait(false);
            
            getResult = LazyResolveResult<TValue>(HasValue, Value);

            return new LazyResolveResult<TValue>(result.IsSuccess == true, result.Value);
        }
        finally
        {
            TryGetSemaphore.Release();
        }
    }

    #endregion

    public virtual async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var result = await GetImpl(cancellationToken).ConfigureAwait(false);
        readCacheValue = result.IsSuccess == true ? result.Value : default;
        return result;
    }

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;
    public ITask<IGetResult<TValue>>? GetState => getState;
    private ITask<IGetResult<TValue>>? getState;


    public ILazyGetResult<TValue> QueryValue() => getResult;
    protected ILazyGetResult<TValue> getResult = new LazyResolveNoopResult<TValue>();

}

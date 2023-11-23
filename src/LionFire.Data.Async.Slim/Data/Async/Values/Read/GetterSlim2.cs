using MorseCode.ITask;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

// TODO: Dupe of GetsSlim? What's the difference?

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// AsyncGets&lt;T&gt; also implements ILazilyGets&lt;T&gt; and provides Rx features. Consider using it.
/// </remarks>
public abstract class GetterSlim2<TValue> : IGetter<TValue>
{
    #region Configuration

    public static bool DisposeValue => GetterOptions<TValue>.Default.DisposeValue;

    #endregion

    #region Value

    public bool HasValue => getResult.IsSuccess();

    public TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get => Value ?? GetIfNeeded().Result.Value;
    }

    public TValue? ReadCacheValue // TODO: get accessor to getResult.Value?
    {
        get => readCacheValue;
        protected set
        {
            if (EqualityComparer<TValue>.Default.Equals(readCacheValue, value)) return;
            var oldValue = readCacheValue;

            // TODO: Move this to a finally block?
            if (DisposeValue && oldValue is IDisposable d)
            {
                Log.Get<GetterSlim2<TValue>>().LogDebug("Disposing object of type {Type}", d.GetType().FullName);
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
        getResult = new NoopGetResult<TValue>()
        {
            //Flags = TransferResultFlags.Discarded, // TODO
        };
    }

    #endregion

    #endregion

    #region GetValue

    private SemaphoreSlim TryGetSemaphore = new SemaphoreSlim(1);

    public async ITask<IGetResult<TValue>> GetIfNeeded()
    {
        await TryGetSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            var lastGetResult = QueryGetResult();
            if (lastGetResult.HasValue)
            {
                if (lastGetResult.IsSuccess()) return lastGetResult;
                //else if (lastGetResult.IsFail()) {  // ENH: return instant fail if too many recent failures, based on options? //}
            }

            var result = getResult = await Get().ConfigureAwait(false);
            return result;
            //return new LazyResolveResult<TValue>(result.IsSuccess == true, result.Value);
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

    public IGetResult<TValue> QueryGetResult() => getResult;
    protected IGetResult<TValue> getResult = new NoopGetResult<TValue>();

    #region TODO: implement somehow
    public IObservable<ITask<IGetResult<TValue>>> GetOperations => getOperations;
    protected BehaviorSubject<ITask<IGetResult<TValue>>> getOperations = new(Task.FromResult<IGetResult<TValue>>(NoopGetResult<TValue>.Instantiated).AsITask());
    #endregion
}

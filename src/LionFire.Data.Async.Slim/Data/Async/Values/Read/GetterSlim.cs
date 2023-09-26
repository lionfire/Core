
using LionFire.ExtensionMethods.Poco.Data.Async;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LionFire.Data.Async.Gets;

/// <summary>
/// Like GetterSlim but uses BehaviorSubject to hold the latest get result.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract class GetterSlimO<TValue> : IGetter<TValue>
{
    //BehaviorSubject<IGetResult<TValue>> getResult = new();
    public TValue? ReadCacheValue => throw new NotImplementedException();

    public TValue? Value => throw new NotImplementedException();

    public bool HasValue => throw new NotImplementedException();

    public void Discard()
    {
        throw new NotImplementedException();
    }

    public void DiscardValue()
    {
        throw new NotImplementedException();
    }

    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ITask<IGetResult<TValue>> GetIfNeeded()
    {
        throw new NotImplementedException();
    }

    public IGetResult<TValue> QueryGetResult()
    {
        throw new NotImplementedException();
    }

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => getOperations;

    protected BehaviorSubject<ITask<IGetResult<TValue>>> getOperations = new(Task.FromResult<IGetResult<TValue>>(NoopGetResult<TValue>.Instantiated).AsITask());
}


///// <summary>
///// Extended functionality from options:
/////  - expiry
/////  - auto-refresh
/////  - auto-get
///// </summary>
///// <typeparam name="TValue"></typeparam>
//public abstract class GetterSlimEx<TValue> : IGets<TValue>
//{
//}

///// <summary>
/////  - after successfully got value, can't re-get
///// </summary>
///// <typeparam name="TValue"></typeparam>
//public abstract class OneShotGetterSlim<TValue> : IGets<TValue>
//{
//}

/// <summary>
/// Slim version of Gets<typeparamref name="TValue"/>.  This slim edition has no events, no Rx subjects, and no ReactiveObject (ReactiveUI) base class.
/// 
/// Handle events, if desired, through: (TODO)
///  - OnGetting
///  - OnGet
///  - OnValueChanged
/// 
/// </summary>
/// <remarks>
/// Only requires one method to be implemented: GetImpl.
/// </remarks>
public abstract class GetterSlim<TValue> : IGetter<TValue>
{
    #region Configuration

    public static bool DisposeValue => GetterOptions<TValue>.Default.DisposeValue;

    #endregion

    #region Lifecycle

    public void Dispose()
    {
        DiscardValue(); // Disposes ReadCacheValue, if any
        tryGetSemaphore = null;
    }

    #endregion

    #region Value

    /// <summary>
    /// True if internal Value field is not default.  If default is a valid value, use DefaultableValue&lt;TValue&gt; as TValue type
    /// </summary>
    public virtual bool HasValue => IsValueDefault;

    public virtual bool IsValueDefault => EqualityComparer.Equals(ReadCacheValue, default);

    public virtual IEqualityComparer<TValue> EqualityComparer => EqualityComparer<TValue>.Default;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public TValue? Value
    {
        [Blocking(Alternative = nameof(TryGetValue))]
        get => ReadCacheValue ?? TryGetValue().Result;
    }

    ILogger l => Log.Get<GetterSlim<TValue>>();

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
                l.LogDebug("Disposing object of type {Type}", d.GetType().FullName);
                d.Dispose();
            }

            readCacheValue = value;

            OnValueChanged(value, oldValue);
        }
    }

    /// <summary>
    /// Raw field for readCacheValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
    /// </summary>
    protected TValue? readCacheValue;

    /// <summary>
    /// Raised when ReadCacheValue changes
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    protected virtual void OnValueChanged(TValue? newValue, TValue? oldValue)
    {
    }

    #endregion

    #region GetValue

    private SemaphoreSlim TryGetSemaphore => tryGetSemaphore ?? throw new ObjectDisposedException(nameof(GetterSlim<TValue>));

    public abstract IObservable<ITask<IGetResult<TValue>>> GetOperations { get; }

    SemaphoreSlim? tryGetSemaphore = new SemaphoreSlim(1);

    public async ITask<IGetResult<TValue>> GetIfNeeded()
    {
        // OPTIMIZE REVIEW - is there an atomic way to avoid the semaphore, at least in some cases?  Use a tuple for (bool HasValue, TValue Value) ?

        await TryGetSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (HasValue)
            {
                var currentValue = ReadCacheValue;
                if (!EqualityComparer.Equals(currentValue, default)) return new NoopGetResult2<TValue>(ReadCacheValue);
            }

            var resolveResult = await Get().ConfigureAwait(false);
            return new GetResult<TValue>(resolveResult.Value, resolveResult.HasValue);
        }
        finally
        {
            TryGetSemaphore.Release();
        }
    }

    public async ValueTask<TValue?> TryGetValue()
    {
        // OPTIMIZE REVIEW - is there an atomic way to avoid the semaphore, at least in some cases?  Use a tuple for (bool HasValue, TValue Value) ?

        await TryGetSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (HasValue)
            {
                var currentValue = ReadCacheValue;
                if (!EqualityComparer.Equals(currentValue, default)) return currentValue;
            }

            var resolveResult = await Get().ConfigureAwait(false);
            return resolveResult.Value;
        }
        finally
        {
            TryGetSemaphore.Release();
        }
    }

    #endregion

    #region QueryValue

    public IGetResult<TValue> QueryGetResult()
    {
        var currentValue = ReadCacheValue;
        return !EqualityComparer<TValue>.Default.Equals(currentValue, default) ? new NoopGetResult2<TValue>(ReadCacheValue) : (IGetResult<TValue>)NotFoundGetResult<TValue>.Instance;
    }

    #endregion

    #region Discard

    public virtual void Discard() => DiscardValue();
    public virtual void DiscardValue()
    {
        if (DisposeValue)
        {
            var oldValue = ReadCacheValue;
            if (oldValue is IDisposable d) d.Dispose();
        }
        ReadCacheValue = default;
    }

    #endregion

    #region Resolve

    public virtual async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var resolveResult = await GetImpl(cancellationToken).ConfigureAwait(false);
        ReadCacheValue = resolveResult.Value;
        Debug.Assert(!IsValueDefault, "GetImpl should not return default, or it will be repeatedly reevaluated.  Consider using GetsDefaultableSlim instead.");
        return resolveResult;
    }

    #endregion

    #region Abstract

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    #endregion

}


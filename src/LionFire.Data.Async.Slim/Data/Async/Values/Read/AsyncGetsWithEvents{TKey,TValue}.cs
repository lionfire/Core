
namespace LionFire.Data.Gets;

/// <summary>
/// Only requires one method to be implemented: GetImpl.
/// </summary>
/// <remarks>
/// TODO: bring this into alignment with the main Gets (base class ReactiveObject)
/// </remarks>
public abstract class AsyncGetsWithEvents<TKey, TValue>
    : DisposableKeyed<TKey>
    , INotifyWrappedValueChanged
    , INotifyWrappedValueReplaced
    , IGets<TValue>
//  - RENAME this class to LazilyResolvesDeluxe<> to distinguish from LazilyResolves<>? Or rename LazilyResolves<> to SimpleLazilyResolves<>?
{
    static ValueChangedPropagation ValueChangedPropagation = new ValueChangedPropagation();

    #region Configuration

    public static bool DisposeValue => AsyncGetOptions<TValue>.Default.DisposeValue;

    #endregion

    #region Lifecycle

    protected AsyncGetsWithEvents() { }
    protected AsyncGetsWithEvents(TKey input) : base(input) { }

    public override void Dispose()
    {
        base.Dispose();
        DiscardValue(); // Disposes ProtectedValue, if any
    }

    #endregion

    #region Value

    /// <summary>
    /// True if internal Value field is not default.  If default is a valid value, use DefaultableValue&lt;TValue&gt; as TValue type
    /// </summary>
    public bool HasValue { get; protected set; }

    public virtual bool IsValueDefault => EqualityComparer.Equals(ReadCacheValue, default);
    public virtual IEqualityComparer<TValue> EqualityComparer => EqualityComparer<TValue>.Default;

    public TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get => ReadCacheValue ?? GetIfNeeded().Result.Value;
    }

    // REVIEW: should this be TValue?  Should TValue have constraint of : default?
    public TValue? ReadCacheValue
    {
        get => protectedValue;
        protected set
        {
            if (EqualityComparer<TValue>.Default.Equals(protectedValue, value)) return;
            var oldValue = protectedValue;

            // TODO: Move this to a finally block?
            if (DisposeValue && oldValue is IDisposable d)
            {
                Log.Get<AsyncGetsWithEvents<TKey, TValue>>().LogDebug("Disposing object of type {Type}", d.GetType().FullName);
                d.Dispose();
            }

            ValueChangedPropagation.Detach(this, protectedValue);
            protectedValue = value;
            ValueChangedPropagation.Attach(this, protectedValue, o => WrappedValueChanged?.Invoke(this));
            WrappedValueForFromTo?.Invoke(this, oldValue, protectedValue);
            WrappedValueChanged?.Invoke(this); // Assume that there was a change

            OnValueChanged(value, oldValue);
        }
    }
    /// <summary>
    /// Raw field for protectedValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
    /// </summary>
    protected TValue? protectedValue;

    public event Action<INotifyWrappedValueReplaced, object?, object?>? WrappedValueForFromTo;
    public event Action<INotifyWrappedValueChanged>? WrappedValueChanged;

    /// <summary>
    /// Raised when ProtectedValue changes
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="oldValue"></param>
    protected virtual void OnValueChanged(TValue? newValue, TValue? oldValue)
    {
        LazilyGetsEvents.RaiseValueChanged<TValue>(this, oldValue, newValue);
    }

    #endregion

    #region GetValue

    private SemaphoreSlim TryGetSemaphore = new SemaphoreSlim(1);

    public async ITask<IGetResult<TValue>> GetIfNeeded()
    {
        if (HasValue) { return new NoopGetResult<TValue>(true, ReadCacheValue); }

        // TODO DUPE - Dedupe in GetsUtils.GetIfNeeded

        await TryGetSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (HasValue) { return new NoopGetResult<TValue>(true, ReadCacheValue); }

            //var currentValue = ReadCacheValue;
            //if (!EqualityComparer<TValue>.Default.Equals(currentValue, default)) return new ResolveResultNoop<TValue>(currentValue);

            var resolveResult = await Get().ConfigureAwait(false);
            return new LazyResolveResult<TValue>(resolveResult.HasValue, resolveResult.Value);
        }
        finally
        {
            TryGetSemaphore.Release();
        }
    }

    #endregion

    #region QueryValue

    public IGetResult<TValue> QueryValue()
    {
        var currentValue = ReadCacheValue;
        return !EqualityComparer<TValue>.Default.Equals(currentValue, default) ? new ResolveResultNoop<TValue>(ReadCacheValue) : (IGetResult<TValue>)ResolveResultNotResolved<TValue>.Instance;
    }

    #endregion

    #region Discard


    public virtual void Discard() => DiscardValue();
    public virtual void DiscardValue()
    {
        ReadCacheValue = default;
        HasValue = false;
    }

    #endregion

    public virtual async Task<bool> Exists()
    {
        if (HasValue) return true;
        var result = await Get().ConfigureAwait(false);
        return result.Flags.IsFound() == true;
    }


    #region Get

    public async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        // 
        var result = await GetImpl(cancellationToken);
        Debug.Assert(result is not null, $"{nameof(GetImpl)} must not return null");
        if (result.IsSuccess == true /*&& result.HasValue*/) // TODO REVIEW: LazilyGets.HasValue should instead be something like IsRetrieved to differentiate whether there is a Value at the source
        {
            HasValue = true;
        } 
        else if (AsyncGetOptions<TValue>.Default.DiscardReadCacheOnGetFailure)
        {
            DiscardValue();
        }
        ReadCacheValue = result.Value;
        return result;
    }

    #endregion

    #region Abstract

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    #endregion

    #region OLD

    //public async ITask<IGetResult<TValueReturned>> GetValue2()
    //{
    //    var currentValue = ProtectedValue;
    //    if (currentValue != null) return new LazyResolveResultNoop<TValueReturned>((TValueReturned)(object)ProtectedValue);

    //    var result = await Get();
    //    return new LazyResolveResult<TValueReturned>(result.HasValue, (TValueReturned)(object)result.Value);
    //}

    #endregion
}

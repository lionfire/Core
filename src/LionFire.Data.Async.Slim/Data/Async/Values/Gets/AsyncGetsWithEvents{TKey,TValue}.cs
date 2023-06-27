
namespace LionFire.Data.Async.Gets;

#if UNUSED

/// <remarks>
/// Only requires one method to be implemented: GetImpl.
/// </remarks>
[Obsolete("Use AsyncGets instead.  Revive this if ReactiveObject for some reason isn't ideal.")]
public abstract class AsyncGetsWithEvents<TKey, TValue>
    : DisposableKeyed<TKey>
    , INotifyWrappedValueChanged
    , INotifyWrappedValueReplaced
    , ILazilyGets<TValue>
//  - RENAME this class to LazilyResolvesDeluxe<> to distinguish from LazilyResolves<>? Or rename LazilyResolves<> to SimpleLazilyResolves<>?
{
    static ValueChangedPropagation ValueChangedPropagation = new ValueChangedPropagation();

    #region Configuration

    public static bool DisposeValue => LazilyGetsOptions<TValue>.DisposeValue;

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
    public virtual bool HasValue => !EqualityComparer<TValue>.Default.Equals(ProtectedValue, default);

    public TValue? Value
    {
        [Blocking(Alternative = nameof(TryGetValue))]
        get => ProtectedValue ?? TryGetValue().Result.Value;
    }

    //SmartWrappedValue SmartWrappedValue = new SmartWrappedValue();
    //protected TValue ProtectedValue { get=>SmartWrappedValue.Prote}

    // REVIEW: should this be TValue?  Should TValue have constraint of : default?
    protected TValue? ProtectedValue
    {
        get => protectedValue;
        set
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

    public async ITask<ILazyGetResult<TValue>> TryGetValue()
    {
        await TryGetSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            var currentValue = ProtectedValue;
            if (!EqualityComparer<TValue>.Default.Equals(currentValue, default)) return new ResolveResultNoop<TValue>(ProtectedValue);

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

    public ILazyGetResult<TValue> QueryValue()
    {
        var currentValue = ProtectedValue;
        return !EqualityComparer<TValue>.Default.Equals(currentValue, default) ? new ResolveResultNoop<TValue>(ProtectedValue) : (ILazyGetResult<TValue>)ResolveResultNotResolved<TValue>.Instance;
    }

    #endregion

    #region Discard


    public virtual void Discard() => DiscardValue();
    public virtual void DiscardValue() => ProtectedValue = default;

    #endregion

    #region Get

    public async ITask<IGetResult<TValue>> Get()
    {
        var result = await GetImpl();
        Debug.Assert(result is not null, $"{nameof(GetImpl)} must not return null");
        ProtectedValue = result.Value;
        return result;
    }

    #endregion

    #region Abstract

    protected abstract ITask<IGetResult<TValue>> GetImpl();

    #endregion

    #region OLD

    //public async ITask<ILazyGetResult<TValueReturned>> GetValue2()
    //{
    //    var currentValue = ProtectedValue;
    //    if (currentValue != null) return new LazyResolveResultNoop<TValueReturned>((TValueReturned)(object)ProtectedValue);

    //    var result = await Get();
    //    return new LazyResolveResult<TValueReturned>(result.HasValue, (TValueReturned)(object)result.Value);
    //}

    #endregion
}

#endif

namespace LionFire.Data.Gets;

/// <remarks>
/// Consider AsyncGets instead, for ReactiveUI support.
/// Only requires one method to be implemented: GetImpl.
/// </remarks>
public abstract class AsyncGetsWithEvents<TValue>
    : INotifyWrappedValueChanged
    , INotifyWrappedValueReplaced
    , ILazilyGets<TValue>
{
    static ValueChangedPropagation ValueChangedPropagation = new ValueChangedPropagation();

    #region Configuration

    public static AsyncGetOptions DefaultOptions => AsyncGetOptions<TValue>.Default;

    #endregion

    #region Lifecycle

    protected AsyncGetsWithEvents() { }

    public void Dispose()
    {
        DiscardValue(); // Disposes ReadCacheValue, if any
    }

    #endregion

    #region Value

    /// <summary>
    /// True if internal Value field is not default.  If default is a valid value, use DefaultableValue&lt;TValue&gt; as TValue type
    /// </summary>
    public virtual bool HasValue => !EqualityComparer<TValue>.Default.Equals(ReadCacheValue, default);

    public TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get => ReadCacheValue ?? GetIfNeeded().Result.Value;
    }

    public TValue? ReadCacheValue
    {
        get => readCacheValue;
        protected set
        {
            if (EqualityComparer<TValue>.Default.Equals(readCacheValue, value)) return;
            var oldValue = readCacheValue;

            // TODO: Move this to a finally block?
            if (DefaultOptions.DisposeValue && oldValue is IDisposable d)
            {
                Log.Get<AsyncGetsWithEvents<TValue>>().LogDebug("Disposing object of type {Type}", d.GetType().FullName);
                d.Dispose();
            }
                        
            ValueChangedPropagation.Detach(this, readCacheValue);
            readCacheValue = value;
            ValueChangedPropagation.Attach(this, readCacheValue, o => WrappedValueChanged?.Invoke(this));
            WrappedValueForFromTo?.Invoke(this, oldValue, readCacheValue);
            WrappedValueChanged?.Invoke(this); // Assume that there was a change

            OnValueChanged(value, oldValue);
        }
    }
    /// <summary>
    /// Raw field for readCacheValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
    /// </summary>
    protected TValue? readCacheValue;

    public event Action<INotifyWrappedValueReplaced, object?, object?>? WrappedValueForFromTo;
    public event Action<INotifyWrappedValueChanged>? WrappedValueChanged;

    /// <summary>
    /// Raised when ReadCacheValue changes
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

    public async ITask<ILazyGetResult<TValue>> GetIfNeeded()
    {
        await TryGetSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            var currentValue = ReadCacheValue;
            if (!EqualityComparer<TValue>.Default.Equals(currentValue, default)) return new ResolveResultNoop<TValue>(ReadCacheValue);

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
        var currentValue = ReadCacheValue;
        return !EqualityComparer<TValue>.Default.Equals(currentValue, default) ? new ResolveResultNoop<TValue>(ReadCacheValue) : (ILazyGetResult<TValue>)ResolveResultNotResolved<TValue>.Instance;
    }

    #endregion

    #region Discard

    public virtual void Discard() => DiscardValue();
    public virtual void DiscardValue()
    {
        if(DefaultOptions.DisposeValue)
        {
            var oldValue = ReadCacheValue;
            if (oldValue is IDisposable d) d.Dispose();
        }
        ReadCacheValue = default;
    }

    #endregion

    #region Get

    public async ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        var resolveResult = await GetImpl(cancellationToken);
        Debug.Assert(resolveResult is not null, "GetImpl must not return null");
        ReadCacheValue = resolveResult.Value;
        return resolveResult;
    }

    #endregion

    #region Abstract

    protected abstract ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default);

    #endregion

}


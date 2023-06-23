
using DynamicData;

namespace LionFire.Data.Async;

public abstract class AsyncGets<TKey, TValue>
    : ReactiveObject
    , ILazilyGetsRx<TValue>
    , IDisposable
    , IKeyed<TKey>
{

    #region Parameters

    #region (static)

    public static AsyncGetOptions DefaultOptions { get; set; } = new();

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

    public AsyncGetOptions Options { get; set; }

    AsyncGetOptions IHasNonNullSettable<AsyncGetOptions>.Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    #endregion

    #endregion

    #region Lifecycle

    protected AsyncGets(TKey key, AsyncValueOptions? options = null)
    {
        this.key = key;
        Options = options ?? DefaultOptions;

        this.ObservableForProperty
        //ValueChangedPropagation.Detach(this, protectedValue);
        //protectedValue = value;
        //ValueChangedPropagation.Attach(this, protectedValue, o => WrappedValueChanged?.Invoke(this));
        
    }    

    #endregion

    #region Value

    [Reactive]
    public TValue? Value { get; private set; }

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

    protected abstract ITask<IGetResult<TValue>> GetImpl();

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;
    public ITask<IGetResult<TValue>>? GetState => getState;

    AsyncGetOptions IHasNonNull<AsyncGetOptions>.Object => throw new NotImplementedException();
    private ITask<IGetResult<TValue>>? getState;

    public virtual async ITask<IGetResult<TValue>> Get()
    {
        var getTask = GetState;
        if (getTask != null && getState.AsTask().IsCompleted) { return await getTask.ConfigureAwait(false); }

        var result = await GetImpl().ConfigureAwait(false);
        Value = result.IsSuccess == true ? result.Value : default;
        HasValue = result.IsSuccess == true;
        return result;
    }

    public async ITask<ILazyGetResult<TValue>> TryGetValue()
    {
        if (HasValue) { return QueryValue(); }
        var result = await Get().ConfigureAwait(false);
        return new LazyResolveResult<TValue>(result.IsSuccess == true, result.Value);
    }

    #endregion
}


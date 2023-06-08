

namespace LionFire.Data.Async;

public abstract class LazilyGetsRx<T> 
    : ReactiveObject
    , ILazilyResolves<T>
    , IHasNonNullSettable<AsyncGetOptions>
{
    #region Parameters

    #region (static)

    public static AsyncGetOptions DefaultOptions { get; set; } = new();

    #endregion

    public AsyncGetOptions Options { get; set; }
    
    AsyncGetOptions IHasNonNullSettable<AsyncGetOptions>.Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    #endregion

    #region Lifecycle

    protected LazilyGetsRx()
    {
        Options = DefaultOptions;
    }
    protected LazilyGetsRx(AsyncValueOptions options)
    {
        Options = options;
    }

    #endregion

    #region Value

    [Reactive]
    public T? Value { get; private set; }

    [Reactive]
    public bool HasValue { get; private set; }
    public ILazyResolveResult<T> QueryValue() => new LazyResolveResult<T>(HasValue, Value);

    public void DiscardValue()
    {
        Value = default;
        HasValue = false;
    }

    #endregion

    #region Get

    protected abstract ITask<IResolveResult<T>> GetImpl();

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;
    public ITask<IResolveResult<T>>? GetState => getState;

    AsyncGetOptions IHasNonNull<AsyncGetOptions>.Object => throw new NotImplementedException();
    private ITask<IResolveResult<T>>? getState;

    public virtual async ITask<IResolveResult<T>> Resolve()
    {
        var getTask = GetState;
        if(getTask != null && getState.AsTask().IsCompleted) { return await getTask.ConfigureAwait(false); }

        var result = await GetImpl().ConfigureAwait(false);
        Value = result.IsSuccess == true ? result.Value : default;
        HasValue = result.IsSuccess == true;
        return result;
    }

    public async ITask<ILazyResolveResult<T>> TryGetValue()
    {
        if (HasValue) { return QueryValue(); }
        var result = await Resolve().ConfigureAwait(false);
        return new LazyResolveResult<T>(result.IsSuccess == true, result.Value);
    }

    #endregion
}


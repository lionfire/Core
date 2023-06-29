
using System.Threading;

namespace LionFire.Data;

public abstract class AsyncGets<TValue>
    : ReactiveObject
    , ILazilyGetsRx<TValue>
{
    #region Parameters

    #region (static)

    public static AsyncGetOptions DefaultOptions { get; set; } = new();

    #endregion

    public AsyncGetOptions Options { get; set; }

    AsyncGetOptions IHasNonNullSettable<AsyncGetOptions>.Object { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public virtual IEqualityComparer<TValue> EqualityComparer =>  AsyncGetOptions<TValue>.DefaultEqualityComparer;

    #endregion

    #region Lifecycle

    protected AsyncGets()
    {
        Options = DefaultOptions;
    }

    protected AsyncGets(AsyncGetOptions? options) {
        Options = options ?? DefaultOptions;
    }

    #endregion

    #region Value

    [Reactive]
    public TValue? ReadCacheValue { get; private set; }

    public TValue? Value
    {
        [Blocking(Alternative = nameof(GetIfNeeded))]
        get => ReadCacheValue ?? GetIfNeeded().Result.Value;
    }

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
        // TODO: Triage logic from AsyncGets_NoBase.Get

        var getTask = GetState;
        if (getTask != null && getState.AsTask().IsCompleted) { return await getTask.ConfigureAwait(false); }

        var result = await GetImpl().ConfigureAwait(false);
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


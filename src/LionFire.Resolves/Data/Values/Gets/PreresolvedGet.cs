#nullable enable

namespace LionFire.Data.Async.Gets;

public record PreresolvedGet<T>(T Value) : ILazilyGets<T>
{
    public bool HasValue => true;

    /// <summary>
    /// Does nothing
    /// </summary>
    public void DiscardValue() { } // => throw new NotSupportedException();

    public ILazyGetResult<T> QueryValue() => new ResolveResultNoop<T>(Value);
    public ITask<ILazyGetResult<T>> GetIfNeeded() => Task.FromResult<ILazyGetResult<T>>(new LazyResolveNoopResult<T>(true, Value)).AsITask();
    ITask<IGetResult<T>> IGets<T>.Get(CancellationToken cancellationToken) => Task.FromResult<IGetResult<T>>(new ResolveResultNoop<T>(Value)).AsITask();

    public Task<T> Get(CancellationToken cancellationToken = default) => Task.FromResult(Value);

    public void Discard()
    {
        DiscardValue();
    }

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

    T? ILazilyGets<T>.ReadCacheValue => Value;
}
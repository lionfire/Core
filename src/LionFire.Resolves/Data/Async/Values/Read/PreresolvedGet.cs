#nullable enable

namespace LionFire.Data.Gets;

public record PreresolvedGet<T>(T Value) : IGets<T>
{
    public bool HasValue => true;

    public void DiscardValue() => throw new NotSupportedException();

    public IGetResult<T> QueryValue() => new ResolveResultNoop<T>(Value);
    public ITask<IGetResult<T>> GetIfNeeded() => Task.FromResult<IGetResult<T>>(new NoopGetResult<T>(true, Value)).AsITask();
    ITask<IGetResult<T>> IStatelessGets<T>.Get(CancellationToken cancellationToken) => Task.FromResult<IGetResult<T>>(new ResolveResultNoop<T>(Value)).AsITask();

    public Task<T> Get(CancellationToken cancellationToken = default) => Task.FromResult(Value);

    public void Discard() => DiscardValue();

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

    T? IGets<T>.ReadCacheValue => Value;
}
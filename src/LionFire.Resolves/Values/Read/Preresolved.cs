#nullable enable

namespace LionFire.Data.Async.Gets;

public record Preresolved<T>(T Value) : ILazilyResolves<T>
{
    public bool HasValue => true;

    /// <summary>
    /// Does nothing
    /// </summary>
    public void DiscardValue() { } // => throw new NotSupportedException();

    public ILazyResolveResult<T> QueryValue() => new ResolveResultNoop<T>(Value);
    public ITask<ILazyResolveResult<T>> TryGetValue() => Task.FromResult<ILazyResolveResult<T>>(new LazyResolveNoopResult<T>(true, Value)).AsITask();
    ITask<IResolveResult<T>> IGets<T>.Resolve() => Task.FromResult<IResolveResult<T>>(new ResolveResultNoop<T>(Value)).AsITask();

    public Task<T> Resolve() => Task.FromResult(Value);
}
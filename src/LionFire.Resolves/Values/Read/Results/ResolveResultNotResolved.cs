using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct ResolveResultNotResolved<TValue> : ISuccessResult, ILazyResolveResult<TValue>
{
    public bool? IsSuccess => false;

    public TValue Value => default;

    public bool HasValue => false;

    public bool IsNoop => false;

    public static ResolveResultNotResolved<TValue> Instance { get; } = new ResolveResultNotResolved<TValue>();
}

using LionFire.Results;

namespace LionFire.Resolves;

public struct ResolveResultNotResolvedNoop<TValue> : ISuccessResult, ILazyResolveResult<TValue>
{
    public bool? IsSuccess => false;

    public TValue Value => default;

    public bool HasValue => false;

    public bool IsNoop => true;

    public static ResolveResultNotResolved<TValue> Instance { get; } = new ResolveResultNotResolved<TValue>();
}

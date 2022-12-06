using LionFire.Results;

namespace LionFire.Resolves;

public struct NoopFailResolveResult<TValue> : ISuccessResult, ILazyResolveResult<TValue>
{
    public bool? IsSuccess => false;
    public bool HasValue => false;
    public TValue Value => default;
    public bool IsNoop => true;

    public static readonly LazyResolveResult<TValue> Instance = new LazyResolveResult<TValue>();

}


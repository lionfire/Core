using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct NoopFailResolveResult<TValue> : ISuccessResult, ILazyGetResult<TValue>, IErrorResult
{
    public static readonly LazyResolveResult<TValue> Instance = new LazyResolveResult<TValue>();

    public bool? IsSuccess => false;
    public bool HasValue => false;
    public TValue Value => default;
    public bool IsNoop => true;

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }


}


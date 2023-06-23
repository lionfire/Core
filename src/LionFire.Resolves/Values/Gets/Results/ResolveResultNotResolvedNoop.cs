using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct ResolveResultNotResolvedNoop<TValue> : ISuccessResult, ILazyGetResult<TValue>
{
    public static ResolveResultNotResolved<TValue> Instance { get; } = new ResolveResultNotResolved<TValue>();

    public bool? IsSuccess => false;

    public TValue Value => default;

    public bool HasValue => false;

    public bool IsNoop => true;


    public TransferResultFlags Flags => TransferResultFlags.Noop | TransferResultFlags.NotFound;

}

using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct NoopNotFoundGetResult<TValue> : IGetResult<TValue>
{
    public static NotFoundGetResult<TValue> Instance { get; } = new NotFoundGetResult<TValue>();

    public readonly bool? IsSuccess => false;

    public readonly TValue Value => default!;

    public readonly bool HasValue => false;

    public readonly bool IsNoop => true;

    public readonly object? Error => null;
    public readonly TransferResultFlags Flags => TransferResultFlags.Noop | TransferResultFlags.NotFound;

}

using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct NoopNotFoundGetResult<TValue> : IGetResult<TValue>
{
    public static NotFoundGetResult<TValue> Instance { get; } = new NotFoundGetResult<TValue>();

    public bool? IsSuccess => false;

    public TValue Value => default;

    public bool HasValue => false;

    public bool IsNoop => true;

    public object? Error => null;
    public TransferResultFlags Flags => TransferResultFlags.Noop | TransferResultFlags.NotFound;

}

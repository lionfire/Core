using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public readonly struct NotFoundGetResult<TValue> : IGetResult<TValue>
{
    public static NotFoundGetResult<TValue> Instance { get; } = new NotFoundGetResult<TValue>();

    public readonly bool? IsSuccess => false;

    public readonly TValue? Value => default;

    public readonly bool HasValue => false;

    public readonly bool IsNoop => false;

    public readonly TransferResultFlags Flags => TransferResultFlags.NotFound | TransferResultFlags.Fail;

    public readonly object? Error => null;

    
}

using LionFire.Results;

namespace LionFire.Data.Gets;

public readonly struct ResolveResultNotResolved<TValue> : ISuccessResult, ILazyGetResult<TValue>
{
    public static ResolveResultNotResolved<TValue> Instance { get; } = new ResolveResultNotResolved<TValue>();

    public readonly bool? IsSuccess => false;

    public readonly TValue? Value => default;

    public readonly bool HasValue => false;

    public readonly bool IsNoop => false;

    public readonly TransferResultFlags Flags => TransferResultFlags.NotFound | TransferResultFlags.Fail;

    public readonly object? Error => null;

    
}

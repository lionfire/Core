namespace LionFire.Data.Async.Gets;

public struct InitializedGetResult<TValue> : IGetResult<TValue>
{
    public static InitializedGetResult<TValue> Instance { get; } = new InitializedGetResult<TValue>();

    public readonly bool? IsSuccess => false;

    public readonly TValue Value => default!;

    public readonly bool HasValue => false;

    public readonly bool IsNoop => true;

    public readonly object? Error => null;
    public readonly TransferResultFlags Flags => TransferResultFlags.Noop | TransferResultFlags.Initialized;
}

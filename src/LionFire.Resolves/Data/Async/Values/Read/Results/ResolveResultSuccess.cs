namespace LionFire.Data.Gets;

public struct ResolveResultSuccess<TValue> : IGetResult<TValue>
{
    public ResolveResultSuccess(TValue value)
    {
        Value = value;
    }

    public bool? IsSuccess => true;

    public TValue Value { get; private set; }

    public bool HasValue => true;

    public TransferResultFlags Flags => TransferResultFlags.Found | TransferResultFlags.Success;
    public object? Error => null;

}

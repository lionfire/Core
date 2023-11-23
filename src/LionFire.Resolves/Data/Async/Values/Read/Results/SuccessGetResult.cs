namespace LionFire.Data.Async.Gets;

public struct SuccessGetResult<TValue> : IGetResult<TValue>
{
    public SuccessGetResult(TValue value)
    {
        Value = value;
    }

    public bool? IsSuccess => true;

    public TValue Value { get; private set; }

    public bool HasValue => true;

    public TransferResultFlags Flags => TransferResultFlags.Found | TransferResultFlags.Success;
    public object? Error => null;

}

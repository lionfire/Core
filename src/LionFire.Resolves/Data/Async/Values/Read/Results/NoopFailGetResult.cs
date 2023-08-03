using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct NoopFailGetResult<TValue> : IGetResult<TValue>
{
    public static readonly NoopFailGetResult<TValue> Instance = new NoopFailGetResult<TValue>();

    public bool? IsSuccess => false;
    public bool HasValue => false;
    public TValue? Value => default;
    public bool IsNoop => true;

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }


}


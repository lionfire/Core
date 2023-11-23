#nullable enable
using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct NoopGetResult2<TValue> : IGetResult<TValue>
{
    /// <summary>
    ///  For default values only
    /// </summary>
    public static NoopGetResult2<TValue> Instance { get; } = new NoopGetResult2<TValue>();

    public NoopGetResult2(TValue? value) { Value = value; }

    //public static implicit operator LazyResolveResult<TValue>(TValue value) => new LazyResolveResult<TValue>(values);

    public bool? IsSuccess => true;
    public bool HasValue => true;
    public TValue? Value { get; set; }
    public bool IsNoop => true;

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

}


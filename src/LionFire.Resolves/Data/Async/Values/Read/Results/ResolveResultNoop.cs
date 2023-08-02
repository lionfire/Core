#nullable enable
using LionFire.Results;

namespace LionFire.Data.Gets;

public struct ResolveResultNoop<TValue> : IGetResult<TValue>
{
    /// <summary>
    ///  For default values only
    /// </summary>
    public static ResolveResultNoop<TValue> Instance { get; } = new ResolveResultNoop<TValue>();

    public ResolveResultNoop(TValue? value) { Value = value; }

    //public static implicit operator LazyResolveResult<TValue>(TValue value) => new LazyResolveResult<TValue>(values);

    public bool? IsSuccess => true;
    public bool HasValue => true;
    public TValue? Value { get; set; }
    public bool IsNoop => true;

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

}



namespace LionFire.Data.Async;

/// <summary>
/// A set is in progress while a get was requested.  Optimistically returning the value used in the set.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class OptimisticGetResult<TValue> : TransferResult, IGetResult<TValue>
{
    public OptimisticGetResult(TValue? value, bool? hasValue = null) : base(TransferResultFlags.Success | TransferResultFlags.PreviewSuccess)
    {
        Value = value;
        HasValue = hasValue ?? !EqualityComparer<TValue>.Default.Equals(value, default);
    }

    public TValue? Value { get; }

    public bool HasValue { get; }
}


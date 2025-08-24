using LionFire.Results;

namespace LionFire.Data.Async.Sets;

public struct SetResult<TValue> : ISetResult<TValue>
{
    #region (Static)

    public static SetResult<TValue> Success(TValue? value = default) => new SetResult<TValue> { Value = value, Flags = TransferResultFlags.Success };
    public static SetResult<TValue> SyncSuccess(TValue? value = default) => new SetResult<TValue> { Value = value, Flags = TransferResultFlags.Success | TransferResultFlags.RanSynchronously };
    public static SetResult<TValue> DeleteSuccess { get; } = new SetResult<TValue> { Flags = TransferResultFlags.Success }; // REVIEW: more explicit flag to say deleted, or no value?
    public static SetResult<TValue> NoopSuccess(TValue? value = default) => new SetResult<TValue> { Value = value, Flags = TransferResultFlags.Success | TransferResultFlags.Noop };
    public static SetResult<TValue> FromException(Exception ex, TValue? value = default) => new SetResult<TValue> { Value = value, Flags = TransferResultFlags.Fail, Error = ex };
    public static SetResult<TValue> FailWithFlags(TransferResultFlags flags) => new SetResult<TValue> { Flags = flags };

    #endregion

    public bool HasValue => this.HasValue();//IValueResultX.HasValue(this);//{ get; set; }

    public TValue? Value { get; set; }
    //public bool IsNoop => false;
    //public SetResult(bool hasValue, TValue? value) { HasValue = hasValue; Value = value; }
    public readonly bool? IsSuccess => Flags.HasFlag(TransferResultFlags.Success);

    ////public static implicit operator LazyResolveResult<TValue>((bool HasValue, TValue Value) values) => new LazyGetResult<TValue>(values.HasValue, values.Value);

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

    public override string ToString()
    {
        return $"SetResult: {(!IsSuccess.HasValue ? "(?)" : (IsSuccess.Value ? "success" : "FAIL"))} - {Flags}";
    }
}
public static class SetResultX
{
    public static ISetResult<TValue> ToSetResult<TValue>(this ITransferResult result, TValue? value = default)
    {
        return new SetResult<TValue>
        {
            Flags = result.Flags,
            Value = value,
        };
    }
}

public struct NoopSetResult<TValue> : ISetResult<TValue>
{
    #region (static)

    public static NoopSetResult<TValue> Instantiated = new NoopSetResult<TValue> { Flags = TransferResultFlags.Instantiated | TransferResultFlags.Noop };

    #endregion

    public NoopSetResult()
    {
    }

    public TransferResultFlags Flags { get; set; } = TransferResultFlags.Noop;

    public bool? IsSuccess { get; set; } = null;

    public TValue? Value => default;

    public bool HasValue => false;
    public object? Error { get; set; }

}

#nullable enable
using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct GetResult<TValue> : IGetResult<TValue>
{
    #region Static

    public static GetResult<TValue> Discarded => new GetResult<TValue>(default, false) { Flags = TransferResultFlags.Discarded | TransferResultFlags.Noop };
    public static GetResult<TValue> Instantiated => new GetResult<TValue>(default, false) { Flags = TransferResultFlags.Instantiated | TransferResultFlags.Noop };
    public static GetResult<TValue> NoopSuccess(TValue value) => new GetResult<TValue>(value, true) { Flags = TransferResultFlags.Success | TransferResultFlags.Noop };
    public static GetResult<TValue> SyncSuccess(TValue value) => new GetResult<TValue>(value, true) { Flags = TransferResultFlags.Success | TransferResultFlags.RanSynchronously };
    public static GetResult<TValue> Exception(Exception exception) => new GetResult<TValue> { Flags = TransferResultFlags.Fail, Error = exception };

    #endregion

    public bool HasValue { get; set; }
    public TValue? Value { get; set; }
    public bool IsNoop => false;
    public GetResult(TValue? value, bool hasValue = true) { HasValue = hasValue; Value = value; }

    //public static implicit operator LazyResolveResult<TValue>((bool HasValue, TValue Value) values) => new LazyGetResult<TValue>(values.HasValue, values.Value);

    public bool? IsSuccess => HasValue;

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

    #region Struct implementation

    public override bool Equals(object? obj)
    {
        if (!(obj is GetResult<TValue> mys)) return false;
        return this.HasValue == mys.HasValue && ReferenceEquals(this.Value, mys.Value);
    }

    public override int GetHashCode() => (Value?.GetHashCode() ?? 0.GetHashCode()) + HasValue.GetHashCode();

    public static bool operator ==(GetResult<TValue> left, GetResult<TValue> right) => left.Equals(right);

    public static bool operator !=(GetResult<TValue> left, GetResult<TValue> right) => !(left == right);

    #endregion

}

//public interface INotifyingLazyGetter // Use Persistence instead?
//{
//    public event Action<ILazilyGets> Resolved;
//    public event Action<ILazilyGets> Discarded;
//}

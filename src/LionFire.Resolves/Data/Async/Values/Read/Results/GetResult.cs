#nullable enable
using LionFire.Data.Async.Sets;
using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct GetResult<TValue> : IGetResult<TValue>
{
    #region Static

    public static GetResult<TValue> Discarded => new GetResult<TValue>(default, false) { Flags = TransferResultFlags.Discarded | TransferResultFlags.Noop };
    public static GetResult<TValue> Instantiated => new GetResult<TValue>(default, false) { Flags = TransferResultFlags.Instantiated | TransferResultFlags.Noop };
    public static GetResult<TValue> NoopSuccess(TValue? value) => new GetResult<TValue>(value, true) { Flags = TransferResultFlags.Success | TransferResultFlags.Noop };

    public static GetResult<TValue> FromSet(ISetResult<TValue> setResult) => new GetResult<TValue>(setResult.Value, setResult.HasValue) { Flags = TransferResultFlags.Success | TransferResultFlags.Set | (setResult.HasValue ? TransferResultFlags.Found : TransferResultFlags.NotFound) };

    public static GetResult<TValue> QueryValue(TValue? value, bool hasValue, TransferResultFlags extraFlags = TransferResultFlags.None)
        => new GetResult<TValue>(value, hasValue)
        {
            Flags =
            TransferResultFlags.Success
            | (hasValue && value is not null ? TransferResultFlags.Found : TransferResultFlags.NotFound)
            | TransferResultFlags.RanSynchronously
            | TransferResultFlags.Noop
            | extraFlags
        };

    public static GetResult<TValue> NoopSyncSuccess(TValue? value, TransferResultFlags extraFlags = TransferResultFlags.None)
        => new GetResult<TValue>(value, true)
        {
            Flags =
            TransferResultFlags.Success
            | (value is not null ? TransferResultFlags.Found : TransferResultFlags.NotFound)
            | TransferResultFlags.RanSynchronously
            | TransferResultFlags.Noop
            | extraFlags
        };

    public static GetResult<TValue> SyncSuccess(TValue? value, TransferResultFlags extraFlags = TransferResultFlags.None) 
        => new GetResult<TValue>(value, true)
        {
            Flags =
            TransferResultFlags.Success
            | (value is not null ? TransferResultFlags.Found : TransferResultFlags.NotFound)
            | TransferResultFlags.RanSynchronously
            | extraFlags
        };

    public static GetResult<TValue> Success(TValue? value, TransferResultFlags extraFlags = TransferResultFlags.None)
        => new GetResult<TValue>(value, true)
        {
            Flags =
            TransferResultFlags.Success
            | (value is not null ? TransferResultFlags.Found : TransferResultFlags.NotFound)
            | extraFlags
        };

    public static GetResult<TValue> Exception(Exception exception) => new GetResult<TValue> { Flags = TransferResultFlags.Fail, Error = exception };

    #endregion

    public bool HasValue { get; set; }
    public TValue? Value { get; set; }
    public bool IsNoop => false;
    private GetResult(TValue? value, bool hasValue = true) { HasValue = hasValue; Value = value; }

    //public static implicit operator LazyResolveResult<TValue>((bool HasValue, TValue Value) values) => new LazyGetResult<TValue>(values.HasValue, values.Value);

    //public bool? IsSuccess => HasValue;
    public bool? IsSuccess => Flags.HasFlag(TransferResultFlags.Success);

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

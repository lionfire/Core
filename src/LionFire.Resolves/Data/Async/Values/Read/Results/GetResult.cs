#nullable enable
using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public struct GetResult<TValue> : IGetResult<TValue>
{
    public bool HasValue { get; set; }
    public TValue? Value { get; set; }
    public bool IsNoop => false;
    public GetResult(bool hasValue, TValue? value) { HasValue = hasValue; Value = value; }

    //public static implicit operator LazyResolveResult<TValue>((bool HasValue, TValue Value) values) => new LazyGetResult<TValue>(values.HasValue, values.Value);

    public bool? IsSuccess => HasValue;

    public TransferResultFlags Flags { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

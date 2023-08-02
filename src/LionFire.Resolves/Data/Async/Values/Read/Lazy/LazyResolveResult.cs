#nullable enable
using LionFire.Results;

namespace LionFire.Data.Gets;

public struct LazyResolveResult<TValue> : IGetResult<TValue>
{
    public bool HasValue { get; set; }
    public TValue? Value { get; set; }
    public bool IsNoop => false;
    public LazyResolveResult(bool hasValue, TValue? value) { HasValue = hasValue; Value = value; }

    //public static implicit operator LazyResolveResult<TValue>((bool HasValue, TValue Value) values) => new LazyResolveResult<TValue>(values.HasValue, values.Value);

    public bool? IsSuccess => HasValue;

    public TransferResultFlags Flags { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public object? Error { get; set; }

    #region Struct implementation

    public override bool Equals(object? obj)
    {
        if (!(obj is LazyResolveResult<TValue> mys)) return false;
        return this.HasValue == mys.HasValue && ReferenceEquals(this.Value, mys.Value);
    }

    public override int GetHashCode() => (Value?.GetHashCode() ?? 0.GetHashCode()) + HasValue.GetHashCode();

    public static bool operator ==(LazyResolveResult<TValue> left, LazyResolveResult<TValue> right) => left.Equals(right);

    public static bool operator !=(LazyResolveResult<TValue> left, LazyResolveResult<TValue> right) => !(left == right);

    #endregion

}

//public interface INotifyingLazilyResolves // Use Persistence instead?
//{
//    public event Action<ILazilyGets> Resolved;
//    public event Action<ILazilyGets> Discarded;
//}

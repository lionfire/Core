#nullable enable

namespace LionFire.Data.Gets;

public struct NoopGetResult<TValue> : IGetResult<TValue>
{
    #region Static

    public static NoopGetResult<TValue> Instantiated { get; } = new NoopGetResult<TValue> { Flags = TransferResultFlags.Instantiated };
    public static NoopGetResult<TValue> Discarded { get; } = new NoopGetResult<TValue> { Flags = TransferResultFlags.Discarded };

    #endregion

    public bool HasValue { get; set; }
    public TValue? Value { get; set; }
    public bool IsNoop => true;
    public NoopGetResult(bool hasValue, TValue? value) { HasValue = hasValue; Value = value; }

    //public static implicit operator LazyResolveResult<TValue>((bool HasValue, TValue Value) values) => new LazyResolveResult<TValue>(values.HasValue, values.Value);

    public bool? IsSuccess => HasValue;

    public TransferResultFlags Flags { get; set; }

    public object? Error { get; set; }

    #region Struct implementation

    public override bool Equals(object? obj)
    {
        if (!(obj is NoopGetResult<TValue> mys)) return false;
        return this.HasValue == mys.HasValue && ReferenceEquals(this.Value, mys.Value);
    }

    public override int GetHashCode() => (Value?.GetHashCode() ?? 0.GetHashCode()) + HasValue.GetHashCode();

    public static bool operator ==(NoopGetResult<TValue> left, NoopGetResult<TValue> right) => left.Equals(right);

    public static bool operator !=(NoopGetResult<TValue> left, NoopGetResult<TValue> right) => !(left == right);

    #endregion

}

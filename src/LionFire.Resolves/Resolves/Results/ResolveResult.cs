using LionFire.Results;

namespace LionFire.Resolves
{
    public static class IResolveResultExtensions
    {
        public static ILazyResolveResult<TValue> ToLazyResolveResult<TValue>(this IResolveResult<TValue> input) 
            => input is ILazyResolveResult<TValue> lrr ? lrr 
            : new LazyResolveResult<TValue>(input.HasValue, input.Value);
    }
    public struct LazyResolveResult<TValue> : ISuccessResult, ILazyResolveResult<TValue>
    {
        public bool HasValue { get; set; }
        public TValue Value { get; set; }
        public bool IsNoop => false;
        public LazyResolveResult(bool hasValue, TValue value) { HasValue = hasValue; Value = value; }

        //public static implicit operator LazyResolveResult<T>((bool HasValue, T Value) values) => new LazyResolveResult<T>(values.HasValue, values.Value);

        public bool? IsSuccess => HasValue;

        #region Struct implementation

        public override bool Equals(object obj)
        {
            if (!(obj is LazyResolveResult<TValue> mys)) return false;
            return this.HasValue == mys.HasValue && ReferenceEquals(this.Value, mys.Value);
        }

        public override int GetHashCode() => Value.GetHashCode() + HasValue.GetHashCode();

        public static bool operator ==(LazyResolveResult<TValue> left, LazyResolveResult<TValue> right) => left.Equals(right);

        public static bool operator !=(LazyResolveResult<TValue> left, LazyResolveResult<TValue> right) => !(left == right);

        #endregion

    }
    public struct NoopFailResolveResult<TValue> : ISuccessResult, ILazyResolveResult<TValue>
    {
        public bool? IsSuccess => false;
        public bool HasValue => false;
        public TValue Value => default;
        public bool IsNoop => true;

        public static readonly LazyResolveResult<TValue> Instance = new LazyResolveResult<TValue>();

    }

    //public interface INotifyingLazilyResolves // Use Persistence instead?
    //{
    //    public event Action<ILazilyResolves> Resolved;
    //    public event Action<ILazilyResolves> Discarded;
    //}
}

using LionFire.Results;

namespace LionFire.Resolves
{
    public struct ResolveResult<TValue> : ISuccessResult, ILazyResolveResult<TValue>
    {
        public bool HasValue { get; set; }
        public TValue Value { get; set; }
        public bool IsNoop => false;
        public ResolveResult(bool hasValue, TValue value) { HasValue = hasValue; Value = value;  }

        //public static implicit operator LazyResolveResult<T>((bool HasValue, T Value) values) => new LazyResolveResult<T>(values.HasValue, values.Value);

        public bool? IsSuccess => HasValue;

        #region Struct implementation

        public override bool Equals(object obj)
        {
            if (!(obj is ResolveResult<TValue> mys)) return false;
            return this.HasValue == mys.HasValue && ReferenceEquals(this.Value, mys.Value);
        }

        public override int GetHashCode() => Value.GetHashCode() + HasValue.GetHashCode();

        public static bool operator ==(ResolveResult<TValue> left, ResolveResult<TValue> right) => left.Equals(right);

        public static bool operator !=(ResolveResult<TValue> left, ResolveResult<TValue> right) => !(left == right);

        #endregion

        

    }
    public struct NoopFailResolveResult<TValue> : ISuccessResult, ILazyResolveResult<TValue>
    {
        public bool? IsSuccess => false;
        public bool HasValue => false;
        public TValue Value => default;
        public bool IsNoop => true;

        public static readonly ResolveResult<TValue> Instance = new ResolveResult<TValue>();
        
    }

    //public interface INotifyingLazilyResolves // Use Persistence instead?
    //{
    //    public event Action<ILazilyResolves> Resolved;
    //    public event Action<ILazilyResolves> Discarded;
    //}
}

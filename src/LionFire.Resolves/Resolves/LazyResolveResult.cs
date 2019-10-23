using LionFire.Results;

namespace LionFire.Resolves
{
    public struct LazyResolveResultNoop<T> : ISuccessResult, ILazyResolveResult<T>
    {
        public LazyResolveResultNoop(T value) { Value = value; }

        //public static implicit operator LazyResolveResult<T>(T value) => new LazyResolveResult<T>(values);

        public bool? IsSuccess => true;
        public bool HasValue => true;
        public T Value { get; set; }
        public bool IsNoop => false;

        public static LazyResolveResultNoop<T> Instance { get; } = new LazyResolveResultNoop<T>();
    }
    public struct LazyResolveResult<T> : ISuccessResult, ILazyResolveResult<T>
    {
        public bool HasValue { get; set; }
        public T Value { get; set; }
        public bool IsNoop => false;
        public LazyResolveResult(bool hasValue, T value) { HasValue = hasValue; Value = value;  }

        //public static implicit operator LazyResolveResult<T>((bool HasValue, T Value) values) => new LazyResolveResult<T>(values.HasValue, values.Value);

        public bool? IsSuccess => HasValue;

        #region Struct implementation

        public override bool Equals(object obj)
        {
            if (!(obj is LazyResolveResult<T> mys)) return false;
            return this.HasValue == mys.HasValue && ReferenceEquals(this.Value, mys.Value);
        }

        public override int GetHashCode() => Value.GetHashCode() + HasValue.GetHashCode();

        public static bool operator ==(LazyResolveResult<T> left, LazyResolveResult<T> right) => left.Equals(right);

        public static bool operator !=(LazyResolveResult<T> left, LazyResolveResult<T> right) => !(left == right);

        #endregion

    }

    //public interface INotifyingLazilyResolves // Use Persistence instead?
    //{
    //    public event Action<ILazilyResolves> Resolved;
    //    public event Action<ILazilyResolves> Discarded;
    //}
}

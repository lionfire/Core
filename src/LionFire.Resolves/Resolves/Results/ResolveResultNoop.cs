using LionFire.Results;

namespace LionFire.Resolves
{
    public struct ResolveResultNoop<TValue> : ISuccessResult, ILazyResolveResult<TValue>
    {
        public ResolveResultNoop(TValue value) { Value = value; }

        //public static implicit operator LazyResolveResult<T>(T value) => new LazyResolveResult<T>(values);

        public bool? IsSuccess => true;
        public bool HasValue => true;
        public TValue Value { get; set; }
        public bool IsNoop => false;

        public static ResolveResultNoop<TValue> Instance { get; } = new ResolveResultNoop<TValue>();
    }
    
}

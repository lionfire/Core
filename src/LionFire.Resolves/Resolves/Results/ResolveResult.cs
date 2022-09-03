using LionFire.Results;

namespace LionFire.Resolves
{
    public static class IResolveResultExtensions
    {
        public static ILazyResolveResult<TValue> ToLazyResolveResult<TValue>(this IResolveResult<TValue> input) 
            => input is ILazyResolveResult<TValue> lrr ? lrr 
            : new LazyResolveResult<TValue>(input.HasValue, input.Value);
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

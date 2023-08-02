namespace LionFire.Data.Gets;

//public static class IGetResultX
//{
// OLD IGetResult is being replaced with IGetResult
//    public static IGetResult<TValue> ToLazyResolveResult<TValue>(this IGetResult<TValue> input)
//        => input is IGetResult<TValue> lrr ? lrr
//        : new LazyResolveResult<TValue>(input.HasValue, input.Value);
//}

//public interface INotifyingLazilyResolves // Use Persistence instead?
//{
//    public event Action<ILazilyGets> Resolved;
//    public event Action<ILazilyGets> Discarded;
//}


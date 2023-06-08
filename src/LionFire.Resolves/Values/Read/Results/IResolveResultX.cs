namespace LionFire.Data.Async.Gets;

public static class IResolveResultX
{
    public static ILazyResolveResult<TValue> ToLazyResolveResult<TValue>(this IResolveResult<TValue> input)
        => input is ILazyResolveResult<TValue> lrr ? lrr
        : new LazyResolveResult<TValue>(input.HasValue, input.Value);
}

//public interface INotifyingLazilyResolves // Use Persistence instead?
//{
//    public event Action<ILazilyResolves> Resolved;
//    public event Action<ILazilyResolves> Discarded;
//}


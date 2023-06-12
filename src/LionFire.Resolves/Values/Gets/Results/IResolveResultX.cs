﻿namespace LionFire.Data.Async.Gets;

public static class IGetResultX
{
    public static ILazyGetResult<TValue> ToLazyResolveResult<TValue>(this IGetResult<TValue> input)
        => input is ILazyGetResult<TValue> lrr ? lrr
        : new LazyResolveResult<TValue>(input.HasValue, input.Value);
}

//public interface INotifyingLazilyResolves // Use Persistence instead?
//{
//    public event Action<ILazilyGets> Resolved;
//    public event Action<ILazilyGets> Discarded;
//}


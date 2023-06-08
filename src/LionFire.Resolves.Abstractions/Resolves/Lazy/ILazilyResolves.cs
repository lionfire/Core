using LionFire.Structures;
using MorseCode.ITask;

namespace LionFire.Data.Async.Gets;

public interface ILazilyResolves<out T> : IGets<T>, ILazilyResolves, IDefaultableReadWrapper<T>
{
    // TODO: RENAME TryGetValue to ResolveIfNeeded
    ITask<ILazyResolveResult<T>> TryGetValue();

    /// <summary>
    /// RENAME to ResolvedResult or Result, make it a property?
    /// REVIEW - change return type to T?  And separate this into LastResolveResult
    /// </summary>
    /// <returns></returns>
    ILazyResolveResult<T> QueryValue();
}


#if UNUSED

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue">If resolving to the default value (such as null) is possible, use a type wrapped with DefaultableValue&lt;T%gt; for TValue</typeparam>
public interface ILazilyResolvesInvariant<TValue> : ILazilyResolves, IResolves<TValue>, IReadWrapper<TValue>
{
    ITask<ILazyResolveResult<TValue>> GetValue();
}

public interface ILazilyResolvesConcrete<TValue> : ILazilyResolves, IResolves<TValue>, IReadWrapper<TValue>
{
    Task<ILazyResolveResult<TValue>> GetValue();
}
#endif

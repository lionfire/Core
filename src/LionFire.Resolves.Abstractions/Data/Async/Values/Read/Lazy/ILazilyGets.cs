using LionFire.Structures;
using MorseCode.ITask;

namespace LionFire.Data.Async.Gets;

public interface ILazilyGets<out T> : IGets<T>, ILazilyGets, IDefaultableReadWrapper<T>
{
    // TODO: RENAME TryGetValue to ResolveIfNeeded
    ITask<ILazyGetResult<T>> TryGetValue();

    /// <summary>
    /// RENAME to ResolvedResult or Result, make it a property?
    /// REVIEW - change return type to T?  And separate this into LastResolveResult
    /// </summary>
    /// <returns></returns>
    ILazyGetResult<T> QueryValue();
}

#if UNUSED

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue">If resolving to the default value (such as null) is possible, use a type wrapped with DefaultableValue&lt;T%gt; for TValue</typeparam>
public interface ILazilyResolvesInvariant<TValue> : ILazilyGets, IGets<TValue>, IReadWrapper<TValue>
{
    ITask<ILazyGetResult<TValue>> GetValue();
}

public interface ILazilyResolvesConcrete<TValue> : ILazilyGets, IGets<TValue>, IReadWrapper<TValue>
{
    Task<ILazyGetResult<TValue>> GetValue();
}
#endif

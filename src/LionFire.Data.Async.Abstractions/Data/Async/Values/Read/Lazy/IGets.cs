
namespace LionFire.Data.Gets;

public interface IGets<out T>  // RENAME IGets
    : IStatelessGets<T>
    , ILazilyGets
    , IDefaultableReadWrapper<T> 
{
    ITask<IGetResult<T>> GetIfNeeded(); // TODO: Add CancellationToken

    /// <summary>
    /// REVIEW - change return type to T?  And separate this into LastResolveResult
    /// </summary>
    /// <returns></returns>
    IGetResult<T> QueryValue(); // RENAME to LatestGetResult, or GetResult, make a get property
    //IGetResult<T> GetResult { get; } // TODO
    

    T? ReadCacheValue { get; }
}

public interface ILazilyGetsValue<T> : IGets<T>
{
    ValueTask<T> GetValueIfNeeded();
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

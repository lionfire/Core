
namespace LionFire.Data.Async.Gets;


[Flags]
public enum ReadState
{
    Unspecified = 0,
    Unloaded = 1 << 0,
    Loading = 1 << 1,

    /// <summary>
    /// Loaded one time.  If kept in sync, Synchronized used instead
    /// </summary>
    Loaded = 1 << 2,
    Synchronized = 1 << 3,
}

#if UNUSED

/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue">If resolving to the default value (such as null) is possible, use a type wrapped with DefaultableValue&lt;T%gt; for TValue</typeparam>
public interface ILazilyResolvesInvariant<TValue> : ILazilyGets, IGets<TValue>, IReadWrapper<TValue>
{
    ITask<IGetResult<TValue>> GetValue();
}

public interface ILazilyResolvesConcrete<TValue> : ILazilyGets, IGets<TValue>, IReadWrapper<TValue>
{
    Task<IGetResult<TValue>> GetValue();
}
#endif

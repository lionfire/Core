

namespace LionFire.Data.Gets;

/// <summary>
/// Marker interface indicating that a IGets&lt;TValue&gt; is likely also present.
/// </summary>
public interface IStatelessGets { }

/// <summary>
/// An interface for directly initiating read-related persistence operations of single objects:
///  - Get
///  - Exists (implemented via extension wrapping Get, or IDetects)
/// 
/// See also: ILazilyGets
/// </summary>
/// <summary>
/// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
/// </summary>
public interface IStatelessGets<out TValue> : IStatelessGets
{
    /// <summary>
    /// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default); 
}

// For Orleans grains: no covariance - REVIEW - is this really needed?
public interface IStatelessGetsG<TValue> : IStatelessGets
{
    /// <summary>
    /// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    Task<IGetResult<TValue>> Get(CancellationToken cancellationToken = default);
}

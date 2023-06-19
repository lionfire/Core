using LionFire.Resolvers;
using MorseCode.ITask;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;


/// <summary>
/// Marker interface indicating that a IGets&lt;TValue&gt; is likely also present.
/// </summary>
public interface IGets { }

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
public interface IGets<out TValue> : IGets
{
    /// <summary>
    /// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    ITask<IGetResult<TValue>> Get(); // TODO: CancelationToken and Timeout?
}

// For Orleans grains: no covariance - REVIEW - is this really needed?
public interface IGetsG<TValue> : IGets
{
    /// <summary>
    /// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    Task<IGetResult<TValue>> Get(); // TODO: CancelationToken and Timeout?
}

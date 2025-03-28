﻿
namespace LionFire.Data.Async.Gets;

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
public interface IStatelessGetter<out TValue> : IGetter // RENAME IStatelessGettable
{
    /// <summary>
    /// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default);
}

public interface IStatelessGetter<in TParam, out TValue> : IGetter
{
    /// <summary>
    /// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    ITask<IGetResult<TValue>> Get(TParam parameter, CancellationToken cancellationToken = default);
}

// For Orleans grains: no covariance - REVIEW - is this really needed?
public interface IStatelessGetterG<TValue> : IGetter
{
    /// <summary>
    /// Get the value for this instance.  If the value was already resolved or provided, this re-resolves the value.  (Use ILazilyGets to avoid unwanted re-resolving.)
    /// </summary>
    /// <returns></returns>
    Task<IGetResult<TValue>> Get(CancellationToken cancellationToken = default);
}

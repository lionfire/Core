using LionFire.Results;
using System;

namespace LionFire.Resolves;

/// <summary>
/// Marker interface for return values from Resolve methods on IResolves and IAsyncResolves.  Use extension methods to inspect details.
/// </summary>
public interface IResolveResult : ISuccessResult { }

public interface IResolveResult<out TValue> : IResolveResult, IValueResult<TValue>, IHasValueResult
{
}


public static class IResolveResultX
{
    public static Exception ToException<TValue>(this IResolveResult<TValue> result)
    {
        if (result.IsSuccess != true)
        {
            return new Exception("Failed to resolve value.");
        }
        else
        {
            return new InvalidOperationException("ToException called when IsSuccess is true.");
        }
    }

}
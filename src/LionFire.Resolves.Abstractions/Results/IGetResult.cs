using LionFire.Results;

namespace LionFire.Data;

/// <summary>
/// Marker interface for return values from Get methods on IGets.  Use extension methods to inspect details.
/// </summary>
public interface IGetResult : ITransferResult { }

public interface IGetResult<out TValue> : IGetResult, IValueResult<TValue>, IHasValueResult
{
}

public static class IGetResultX
{
    public static Exception ToException<TValue>(this IGetResult<TValue> result)
    {
        if (result.IsSuccess != true)
        {
            return new Exception("Failed to get value.");
        }
        else
        {
            return new InvalidOperationException("ToException called when IsSuccess is true.");
        }
    }
}
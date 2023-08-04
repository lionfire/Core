using LionFire.Results;

namespace LionFire.Data.Async.Sets;

/// <summary>
/// Marker interface for return values from Set methods on ISetter.  (FUTURE: Use extension methods to inspect details, like with IGetResult)
/// </summary>
public interface ISetResult : ITransferResult, IErrorResult { }

public interface ISetResult<out TValue> : ISetResult, IValueResult<TValue>
{
}

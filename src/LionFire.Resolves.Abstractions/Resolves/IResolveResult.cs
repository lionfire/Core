using LionFire.Results;

namespace LionFire.Resolves
{
    /// <summary>
    /// Marker interface for return values from Resolve methods on IResolves and IAsyncResolves.  Use extension methods to inspect details.
    /// </summary>
    public interface IResolveResult : IResult, ISuccessResult { }

    public interface IResolveResult<out TValue> : IResolveResult, IValueResult<TValue>, IHasValueResult { }
}

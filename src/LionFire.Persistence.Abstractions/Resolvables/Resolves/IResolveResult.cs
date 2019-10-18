using LionFire.Results;

namespace LionFire.Resolvables
{
    /// <summary>
    /// Marker interface for return values from Resolve methods on IResolves and IAsyncResolves.  Use extension methods to inspect details.
    /// </summary>
    public interface IResolveResult : IResult { }
}

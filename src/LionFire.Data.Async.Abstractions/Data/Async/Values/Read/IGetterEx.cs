namespace LionFire.Data.Async.Gets;

public interface IGetterEx : IGetter
{
    /// <summary>
    /// Determine whether resolving the Value is a potentially expensive operation (involving blocking I/O, or something computationally expensive.)
    /// If this interface is not present, the answer is assumed to be true (otherwise it would probably not need to be an IAsyncResolvable.)
    /// </summary>
    bool IsResolutionExpensive { get; }
}

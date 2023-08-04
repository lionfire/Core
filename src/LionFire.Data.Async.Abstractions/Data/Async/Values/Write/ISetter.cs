
namespace LionFire.Data.Async.Sets;

public interface ISetter<in TValue>
{
    /// <summary>
    /// Set the current Value to value, and initiate a Put to the underlying data store with that value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ITask<ISetResult<T>> Set<T>(T? value, CancellationToken cancellationToken = default) where T : TValue;
}

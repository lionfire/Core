
namespace LionFire.Data.Async.Gets;

public abstract class CachingGets<T> : ICachingResolves<T>
{
    public T Value => value;
    protected T? value;

    public bool HasValue => value != null;

    protected abstract ITask<IGetResult<T>> GetImpl();

    public virtual async ITask<IGetResult<T>> Get(CancellationToken cancellationToken = default)
    {
        var result = await GetImpl().ConfigureAwait(false);
        value = result.IsSuccess == true ? result.Value : default;
        return result;
    }
}

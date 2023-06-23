using MorseCode.ITask;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

// DUPE - near Duplicate of LazilyGetsRx<T>, sans ReactiveUI

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// AsyncGetsWithEvents&lt;T&gt; also implements ILazilyGets&lt;T&gt; and provides extra features. Consider using it.
/// </remarks>
public abstract class LazilyResolves<T> : ILazilyGets<T>
{
    public T Value => value;
    protected T value;

    public bool HasValue { get; protected set; }

    public virtual async ITask<IGetResult<T>> Get()
    {
        var result = await GetImpl().ConfigureAwait(false);
        value = result.IsSuccess == true ? result.Value : default;
        HasValue = result.IsSuccess == true;
        return result;
    }

    protected abstract ITask<IGetResult<T>> GetImpl();

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;
    public ITask<IGetResult<T>>? GetState => getState;
    private ITask<IGetResult<T>>? getState;

    public async ITask<ILazyGetResult<T>> TryGetValue()
    {
        if (HasValue) { return QueryValue(); }
        var result = await Get().ConfigureAwait(false);
        return new LazyResolveResult<T>(result.IsSuccess == true, result.Value);
    }
    public ILazyGetResult<T> QueryValue() => new LazyResolveResult<T>(HasValue, value);

    public virtual void Discard() => DiscardValue();
    public void DiscardValue()
    {
        value = default;
        HasValue = false;
    }
}

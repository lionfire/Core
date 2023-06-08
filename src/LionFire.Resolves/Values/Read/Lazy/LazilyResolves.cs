using MorseCode.ITask;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

// DUPE - near Duplicate of LazilyGetsRx<T>, sans ReactiveUI

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Resolves&lt;T&gt; also implements ILazilyResolves&lt;T&gt; and provides extra features. Consider using it.
/// </remarks>
public abstract class LazilyResolves<T> : ILazilyResolves<T>
{
    public T Value => value;
    protected T value;

    public bool HasValue { get; protected set; }

    protected abstract ITask<IResolveResult<T>> ResolveImpl();

    public virtual async ITask<IResolveResult<T>> Resolve()
    {
        var result = await ResolveImpl().ConfigureAwait(false);
        value = result.IsSuccess == true ? result.Value : default;
        HasValue = result.IsSuccess == true;
        return result;
    }

    protected abstract ITask<IResolveResult<T>> GetImpl();

    public bool IsGetting => getState != null && getState.AsTask().IsCompleted == false;
    public ITask<IResolveResult<T>>? GetState => getState;
    private ITask<IResolveResult<T>>? getState;

    public async ITask<ILazyResolveResult<T>> TryGetValue()
    {
        if (HasValue) { return QueryValue(); }
        var result = await Resolve().ConfigureAwait(false);
        return new LazyResolveResult<T>(result.IsSuccess == true, result.Value);
    }
    public ILazyResolveResult<T> QueryValue() => new LazyResolveResult<T>(HasValue, value);

    public void DiscardValue()
    {
        value = default;
        HasValue = false;
    }
}

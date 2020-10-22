using MorseCode.ITask;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
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

}

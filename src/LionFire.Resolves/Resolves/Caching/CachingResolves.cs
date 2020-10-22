using MorseCode.ITask;

namespace LionFire.Resolves
{
    public abstract class CachingResolves<T> : ICachingResolves<T>
    {
        public T Value => value;
        protected T value;

        public bool HasValue => value != null;

        protected abstract ITask<IResolveResult<T>> ResolveImpl();

        public virtual async ITask<IResolveResult<T>> Resolve()
        {
            var result = await ResolveImpl().ConfigureAwait(false);
            value = result.IsSuccess == true ? result.Value : default;
            return result;
        }
    }

}

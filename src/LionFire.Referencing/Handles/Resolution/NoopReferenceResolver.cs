using System;
using System.Threading.Tasks;

namespace LionFire.Referencing.Resolution
{
    public class NoopReferenceResolver : IHandleResolver
    {
        public Task<ResolveHandleResult<T>> Resolve<T>(IReadHandle<T> handle)
            where T : class
        {
            return Task.FromResult(ResolveHandleResult<T>.Unsuccessful);
        }
    }
}

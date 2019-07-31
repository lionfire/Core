using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Referencing.Persistence
{
    public class ExactReferenceResolutionStrategy : IReferenceResolutionStrategy
    {

        public Task<IEnumerable<ReadResolutionResult<T>>> ResolveAll<T>(IReference r, ResolveOptions options = null)
            => Task.FromResult((IEnumerable<ReadResolutionResult<T>>)new ReadResolutionResult<T>[] { new ReadResolutionResult<T>(r) });
        public Task<IEnumerable<WriteResolutionResult<T>>> ResolveAllForWrite<T>(IReference r, ResolveOptions options = null)
            => Task.FromResult((IEnumerable<WriteResolutionResult<T>>)new WriteResolutionResult<T>[] { new WriteResolutionResult<T>(r) });

    }
}

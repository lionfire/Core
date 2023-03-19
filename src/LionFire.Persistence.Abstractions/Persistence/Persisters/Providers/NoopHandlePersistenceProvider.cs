#nullable enable

using LionFire.Referencing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public class NoopPersister<TReference> : IReferencePersister<TReference>
         where TReference : IReference
    {
        public Task<IPersistenceResult> Create<TValue>(TReference reference, TValue value)
            => Task.FromResult<IPersistenceResult>(NoopPutPersistenceResult.Instance);
        public Task<IPersistenceResult> Delete(TReference reference)  
            => Task.FromResult<IPersistenceResult>(NoopPutPersistenceResult.Instance);


        public Task<IPersistenceResult> Exists<TValue>(TReference reference) => Task.FromResult<IPersistenceResult>(PersistenceResult.NotFound);
        public Task<IRetrieveResult<IEnumerable<IListing<T>>>> List<T>(TReference reference, ListFilter? filter = null) 
            => Task.FromResult<IRetrieveResult<IEnumerable<IListing<T>>>>(RetrieveResult<IEnumerable<IListing<T>>>.Noop(Enumerable.Empty<IListing<T>>()));

        public Task<IPersistenceResult> Put<TValue>(TReference reference, TValue value) 
            => Task.FromResult<IPersistenceResult>(NoopPutPersistenceResult.Instance);
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(TReference reference, RetrieveOptions? options = null)
            => Task.FromResult<IRetrieveResult<TValue>>(NoopRetrieveResult<TValue>.Instance);
            
        public Task<IPersistenceResult> Update<TValue>(TReference reference, TValue value) 
            => Task.FromResult<IPersistenceResult>(NoopPutPersistenceResult.Instance);
        public Task<IPersistenceResult> Upsert<TValue>(TReference reference, TValue value) 
            => Task.FromResult<IPersistenceResult>(NoopPutPersistenceResult.Instance);
        
    }
}

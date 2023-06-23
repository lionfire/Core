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
        public Task<ITransferResult> Create<TValue>(TReference reference, TValue value)
            => Task.FromResult<ITransferResult>(NoopPutPersistenceResult.Instance);
        public Task<ITransferResult> Delete(TReference reference)  
            => Task.FromResult<ITransferResult>(NoopPutPersistenceResult.Instance);


        public Task<ITransferResult> Exists<TValue>(TReference reference) => Task.FromResult<ITransferResult>(TransferResult.NotFound);
        public Task<IRetrieveResult<IEnumerable<IListing<T>>>> List<T>(TReference reference, ListFilter? filter = null) 
            => Task.FromResult<IRetrieveResult<IEnumerable<IListing<T>>>>(RetrieveResult<IEnumerable<IListing<T>>>.Noop(Enumerable.Empty<IListing<T>>()));

        public Task<ITransferResult> Put<TValue>(TReference reference, TValue value) 
            => Task.FromResult<ITransferResult>(NoopPutPersistenceResult.Instance);
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(TReference reference, RetrieveOptions? options = null)
            => Task.FromResult<IRetrieveResult<TValue>>(NoopRetrieveResult<TValue>.Instance);
            
        public Task<ITransferResult> Update<TValue>(TReference reference, TValue value) 
            => Task.FromResult<ITransferResult>(NoopPutPersistenceResult.Instance);
        public Task<ITransferResult> Upsert<TValue>(TReference reference, TValue value) 
            => Task.FromResult<ITransferResult>(NoopPutPersistenceResult.Instance);
        
    }
}

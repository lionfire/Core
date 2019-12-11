using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public class PersisterFromReferencePersister<TReference> : IPersister<TReference>
        where TReference : IReference
    {
        public IReferencePersister<TReference> Persister { get; }

        public PersisterFromReferencePersister(IReferencePersister<TReference> persister)
        {
            Persister = persister;
        }

        public Task<IPersistenceResult> Create<TValue>(IReferencable<TReference> referencable, TValue value)
            => Persister.Create(referencable.Reference, value);

        public Task<IPersistenceResult> Exists(IReferencable<TReference> referencable)
            => Persister.Exists(referencable.Reference);

        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable)
            => Persister.Retrieve<TValue>(referencable.Reference);

        public Task<IPersistenceResult> Update<TValue>(IReferencable<TReference> referencable, TValue value)
            => Persister.Update(referencable.Reference, value);

        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value)
            => Persister.Upsert(referencable.Reference, value);

        public Task<IPersistenceResult> Delete(IReferencable<TReference> referencable)
            => Persister.Delete(referencable.Reference);
    }
}

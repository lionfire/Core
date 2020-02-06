#nullable enable
using LionFire.Persistence;
using LionFire.Referencing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public class CurriedPersister<TReference, TOverlayableReference> : IPersister<TReference>
        where TReference : IReference
    {
        public IOverlayableReference<TReference> BaseReference { get; }

        public IPersister<TReference> Persister { get; }

        public CurriedPersister(IPersister<TReference> persister, IOverlayableReference<TReference> baseReference)
        {
            this.Persister = persister;
            this.BaseReference = baseReference;
        }

        public Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable) => Persister.Exists<TValue>(BaseReference.AddRight(referencable.Reference));
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable) => Persister.Retrieve<TValue>(BaseReference.AddRight(referencable.Reference));
        public Task<IPersistenceResult> Create<TValue>(IReferencable<TReference> referencable, TValue value) => Persister.Create(BaseReference.AddRight(referencable.Reference), value);
        public Task<IPersistenceResult> Update<TValue>(IReferencable<TReference> referencable, TValue value) => Persister.Update(BaseReference.AddRight(referencable.Reference), value);
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value) => Persister.Upsert(BaseReference.AddRight(referencable.Reference), value);
        public Task<IPersistenceResult> Delete(IReferencable<TReference> referencable) => Persister.Delete(BaseReference.AddRight(referencable.Reference));
        public Task<IRetrieveResult<IEnumerable<string>>> List(IReferencable<TReference> referencable, ListFilter? filter = null) => Persister.List(referencable, filter);
    }
}

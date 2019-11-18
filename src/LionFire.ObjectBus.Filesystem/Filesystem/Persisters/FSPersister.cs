using LionFire.Dependencies;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Serialization;
using System;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.Filesystem
{
    public class FSPersister : IPersister
    {
        public async Task<IPersistenceResult> Create<TReference, TValue>(IReferencable<TReference> referencable, TValue value)
            where TReference : IReference 
            => await FsOBasePersistence.Write(value, referencable.Reference.Path, typeof(TValue), false).ConfigureAwait(false);

        public Task<IPersistenceResult> Delete<TReference>(IReferencable<TReference> referencable)
            where TReference : IReference
            => throw new NotImplementedException();


        public Task<IExistsResult> Exists<TReference>(IReferencable<TReference> referencable)
            where TReference : IReference
            => throw new NotImplementedException();

        public Task<IRetrieveResult<TValue>> Retrieve<TReference, TValue>(IReferencable<TReference> referencable)
            where TReference : IReference
            => throw new NotImplementedException();

        public Task<IPersistenceResult> Update<TReference, TValue>(IReferencable<TReference> referencable, TValue value)
            where TReference : IReference
            => throw new NotImplementedException();

        public Task<IPersistenceResult> Upsert<TReference, TValue>(IReferencable<TReference> referencable, TValue value)
            where TReference : IReference
            => throw new NotImplementedException();
    }
}

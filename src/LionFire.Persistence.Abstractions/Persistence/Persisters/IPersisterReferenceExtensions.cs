#nullable enable

using LionFire.Persistence;
using LionFire.Referencing;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    public static class IPersisterReferenceExtensions
    {
        public static Task<IPersistenceResult> Create<TReference, TValue>(this IPersister<TReference> persister, TReference reference, TValue value)
            where TReference : IReference
            => persister.Create(new ReferenceWrapper<TReference>(reference), value);

        public static Task<IPersistenceResult> Delete<TReference, TValue>(this IPersister<TReference> persister, TReference reference) 
            where TReference : IReference
            => persister.Delete(new ReferenceWrapper<TReference>(reference));

        public static Task<IPersistenceResult> Exists<TReference, TValue>(this IPersister<TReference> persister, TReference reference) 
            where TReference : IReference
            => persister.Exists<TValue>(new ReferenceWrapper<TReference>(reference));


        public static Task<IRetrieveResult<TValue>> Retrieve<TReference, TValue>(this IPersister<TReference> persister, TReference reference, RetrieveOptions? options = null) 
            where TReference : IReference
            => persister.Retrieve<TValue>(new ReferenceWrapper<TReference>(reference), options);

        public static Task<IPersistenceResult> Update<TReference, TValue>(this IPersister<TReference> persister, TReference reference, TValue value)
            where TReference : IReference
            => persister.Update(new ReferenceWrapper<TReference>(reference), value);

        public static Task<IPersistenceResult> Upsert<TReference, TValue>(this IPersister<TReference> persister, TReference reference, TValue value) 
            where TReference : IReference
            => persister.Upsert(new ReferenceWrapper<TReference>(reference), value);
    }
}

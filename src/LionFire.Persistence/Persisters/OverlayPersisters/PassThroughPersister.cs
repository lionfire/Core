using LionFire.Referencing;
using LionFire.Serialization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{
    /// <summary>
    /// Maps one reference system to another, but otherwise passes through persistence methods to an underlying Persister.
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TUnderlyingReference"></typeparam>
    /// <typeparam name="TUnderlyingPersister"></typeparam>
    public abstract class PassthroughPersister<TReference, TOptions, TUnderlyingReference, TUnderlyingPersister> : SerializingPersisterBase<TOptions>, IPersister<TReference>
        where TReference : IReference
        where TOptions : PersistenceOptions
        where TUnderlyingPersister : class, IPersister<TUnderlyingReference>
        where TUnderlyingReference : IReference, IReferencable<TUnderlyingReference>
    {

        public abstract TUnderlyingReference TranslateReferenceForRead(TReference reference);
        public abstract TUnderlyingReference TranslateReferenceForWrite(TReference reference);
        public virtual TReference ReverseTranslateReference(TUnderlyingReference reference) => throw new NotSupportedException();

        public TUnderlyingPersister UnderlyingPersister
        {
            get
            {
                if (underlyingPersister == null)
                {
                    underlyingPersister = GetUnderlyingPersister;
                }
                return underlyingPersister;
            }
            protected set { underlyingPersister = value; }
        }
        private TUnderlyingPersister underlyingPersister;

        public PassthroughPersister(SerializationOptions serializationOptions) : base(serializationOptions)
        {
        }

        protected virtual TUnderlyingPersister GetUnderlyingPersister => default;


        public Task<IPersistenceResult> Create<TValue>(IReferencable<TReference> referencable, TValue value)
            => UnderlyingPersister.Create(TranslateReferenceForWrite(referencable.Reference), value);

        public Task<IPersistenceResult> Delete(IReferencable<TReference> referencable)
           => UnderlyingPersister.Delete(TranslateReferenceForWrite(referencable.Reference));
        public Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable)
          => UnderlyingPersister.Exists<TValue>(TranslateReferenceForRead(referencable.Reference));
        public Task<IRetrieveResult<IEnumerable<Listing<T>>>> List<T>(IReferencable<TReference> referencable, ListFilter filter = null)
          => UnderlyingPersister.List<T>(TranslateReferenceForRead(referencable.Reference), filter);

        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable)
            => UnderlyingPersister.Retrieve<TValue>(TranslateReferenceForRead(referencable.Reference));

        public Task<IPersistenceResult> Update<TValue>(IReferencable<TReference> referencable, TValue value)
        => UnderlyingPersister.Update(TranslateReferenceForWrite(referencable.Reference), value);
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value)
         => UnderlyingPersister.Upsert(TranslateReferenceForWrite(referencable.Reference), value);
    }
}

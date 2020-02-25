using LionFire.Referencing;
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
    public abstract class PassThroughPersister<TReference, TOptions, TUnderlyingReference, TUnderlyingPersister> : PersisterBase<TOptions>, IPersister<TReference>
        where TReference : IReference
        where TOptions : PersistenceOptions
        where TUnderlyingPersister : IPersister<TUnderlyingReference>
        where TUnderlyingReference : IReference, IReferencable<TUnderlyingReference>
    {

        public abstract TUnderlyingReference TranslateReference(TReference reference);
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
        protected abstract TUnderlyingPersister GetUnderlyingPersister { get; }


        public Task<IPersistenceResult> Create<TValue>(IReferencable<TReference> referencable, TValue value)
            => UnderlyingPersister.Create(TranslateReference(referencable.Reference), value);

        public Task<IPersistenceResult> Delete(IReferencable<TReference> referencable)
           => UnderlyingPersister.Delete(TranslateReference(referencable.Reference));
        public Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable)
          => UnderlyingPersister.Exists<TValue>(TranslateReference(referencable.Reference));
        public Task<IRetrieveResult<IEnumerable<Listing>>> List(IReferencable<TReference> referencable, ListFilter filter = null)
          => UnderlyingPersister.List(TranslateReference(referencable.Reference), filter);

        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable)
            => UnderlyingPersister.Retrieve<TValue>(TranslateReference(referencable.Reference));

        public Task<IPersistenceResult> Update<TValue>(IReferencable<TReference> referencable, TValue value)
        => UnderlyingPersister.Update(TranslateReference(referencable.Reference), value);
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value)
         => UnderlyingPersister.Upsert(TranslateReference(referencable.Reference), value);
    }
}

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
    public abstract class PassthroughPersister<TReference, TOptions, TUnderlyingReference, TUnderlyingPersister> 
        : SerializingPersisterBase<TOptions>
        , IPersister<TReference>
        , IReferenceTranslator<TReference, TUnderlyingReference>
        where TReference : IReference
        where TOptions : PersistenceOptions
        where TUnderlyingPersister : class, IPersister<TUnderlyingReference>
        where TUnderlyingReference : IReference, IReferencable<TUnderlyingReference>
    {

        public abstract TUnderlyingReference TranslateReferenceForRead(TReference reference);
        public abstract TUnderlyingReference TranslateReferenceForWrite(TReference reference);
        public virtual TReference ReverseTranslateReference(TUnderlyingReference reference) => throw new NotSupportedException();

        //public TUnderlyingPersister UnderlyingPersister
        //{
        //    get
        //    {
        //        if (underlyingPersister == null)
        //        {
        //            underlyingPersister = GetUnderlyingPersister;
        //        }
        //        return underlyingPersister;
        //    }
        //    protected set { underlyingPersister = value; }
        //}
        //private TUnderlyingPersister underlyingPersister;

        public PassthroughPersister(SerializationOptions serializationOptions) : base(serializationOptions)
        {
        }

        //protected virtual TUnderlyingPersister GetUnderlyingPersister => default;
        protected virtual TUnderlyingPersister GetUnderlyingPersister(TReference reference) => default;


        public Task<IPersistenceResult> Create<TValue>(IReferencable<TReference> referencable, TValue value)
            => GetUnderlyingPersister(referencable.Reference).Create(TranslateReferenceForWrite(referencable.Reference), value);

        public Task<IPersistenceResult> Delete(IReferencable<TReference> referencable)
           => GetUnderlyingPersister(referencable.Reference).Delete(TranslateReferenceForWrite(referencable.Reference));
        public Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable)
          => GetUnderlyingPersister(referencable.Reference).Exists<TValue>(TranslateReferenceForRead(referencable.Reference));
        public Task<IRetrieveResult<IEnumerable<IListing<T>>>> List<T>(IReferencable<TReference> referencable, ListFilter filter = null)
          => GetUnderlyingPersister(referencable.Reference).List<T>(TranslateReferenceForRead(referencable.Reference), filter);

        public async Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable, RetrieveOptions? options = null)
        {
            var result = await GetUnderlyingPersister(referencable.Reference).Retrieve<TValue>(TranslateReferenceForRead(referencable.Reference), options).ConfigureAwait(false);

            if (result.HasValue && result is INotifyReferenceDeserialized<TReference> nrd)
            {
                nrd.OnDeserialized(referencable.Reference);
            }
            if (referencable is INotifyDeserialized nd)
            {
                nd.OnDeserialized();
            }

            return result;
        }

        public Task<IPersistenceResult> Update<TValue>(IReferencable<TReference> referencable, TValue value)
        => GetUnderlyingPersister(referencable.Reference).Update(TranslateReferenceForWrite(referencable.Reference), value);
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value)
         => GetUnderlyingPersister(referencable.Reference).Upsert(TranslateReferenceForWrite(referencable.Reference), value);
    }
}

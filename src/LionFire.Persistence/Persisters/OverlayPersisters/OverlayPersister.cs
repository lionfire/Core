#nullable enable
using LionFire.Data;
using LionFire.Referencing;
using LionFire.Serialization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters
{

#warning NEXT: determining candidate paths should be an async.  But I am not sure I want to unwrap a Task.FromResult(singlePath) for the simple case.


    /// <summary>
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TUnderlyingPersister"></typeparam>
    /// <remarks>
    /// Examples: AutoExtensionPersister, (perhaps Vos itself if it can be genericized enough.)
    /// </remarks>
    public class OverlayPersister<TReference, TOptions, TUnderlyingPersister> : SerializingPersisterBase<TOptions>, IPersister<TReference>
        where TReference : IReference
        where TOptions : PersistenceOptions
    {
        public OverlayPersister(IServiceProvider serviceProvider, SerializationOptions serializationOptions) : base(serviceProvider, serializationOptions)
        {
        }

        public Task<ITransferResult> Create<TValue>(IReferenceable<TReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<ITransferResult> DeleteReferenceable(IReferenceable<TReference> referencable) => throw new System.NotImplementedException();
        public Task<ITransferResult> Exists<TValue>(IReferenceable<TReference> referencable) => throw new System.NotImplementedException();
        public Task<IGetResult<IEnumerable<IListing<T>>>> List<T>(IReferenceable<TReference> referencable, ListFilter filter = null) => throw new System.NotImplementedException();
        public Task<IGetResult<TValue>> Retrieve<TValue>(IReferenceable<TReference> referencable, RetrieveOptions? options = null) => throw new System.NotImplementedException();
        public Task<ITransferResult> Update<TValue>(IReferenceable<TReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<ITransferResult> Upsert<TValue>(IReferenceable<TReference> referencable, TValue value) => throw new System.NotImplementedException();
    }
}

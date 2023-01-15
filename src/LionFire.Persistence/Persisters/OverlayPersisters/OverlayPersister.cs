﻿#nullable enable
using LionFire.Referencing;
using LionFire.Serialization;
using Microsoft.Extensions.Options;
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
        public OverlayPersister(SerializationOptions serializationOptions) : base(serializationOptions)
        {
        }

        public Task<IPersistenceResult> Create<TValue>(IReferencable<TReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Delete(IReferencable<TReference> referencable) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Exists<TValue>(IReferencable<TReference> referencable) => throw new System.NotImplementedException();
        public Task<IRetrieveResult<IEnumerable<Listing<T>>>> List<T>(IReferencable<TReference> referencable, ListFilter filter = null) => throw new System.NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<TReference> referencable, RetrieveOptions? options = null) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Update<TValue>(IReferencable<TReference> referencable, TValue value) => throw new System.NotImplementedException();
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<TReference> referencable, TValue value) => throw new System.NotImplementedException();
    }
}

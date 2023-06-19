﻿#if UNUSED
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.RegisteredObjects
{

    public class ObjectRegistryPersisterProvider : IPersisterProvider<NamedObjectReference>
    {
        public bool HasDefaultPersister => throw new NotImplementedException();

        public IPersister<NamedObjectReference> GetPersister(string name = null) => throw new NotImplementedException();
    }

    public class ObjectRegistryPersister : IReadPersister<NamedObjectReference>, IWritePersister<NamedObjectReference>
    {
        #region Construction

        public ObjectRegistryPersister() { }
        public ObjectRegistryPersister(NamedObjectsByType objectRegistry) { ObjectRegistry = objectRegistry; }

        #endregion

        public NamedObjectsByType ObjectRegistry { get; set; }

        public Task<ITransferResult> Create<TValue>(IReferencable<NamedObjectReference> referencable, TValue value)
        {
            return Task.FromResult<ITransferResult>(ObjectRegistry.TryAdd(value, referencable.Reference.Path)
                ? PersistenceResult.Success
                : PersistenceResult.FailAndFound);
        }

        public Task<ITransferResult> Delete(IReferencable<NamedObjectReference> referencable) => throw new NotImplementedException();
        public Task<ITransferResult> Exists<TValue>(IReferencable<NamedObjectReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<NamedObjectReference> referencable) => throw new NotImplementedException();
        public Task<ITransferResult> Update<TValue>(IReferencable<NamedObjectReference> referencable, TValue value) => throw new NotImplementedException();
        public Task<ITransferResult> Upsert<TValue>(IReferencable<NamedObjectReference> referencable, TValue value) => throw new NotImplementedException();
    }

    public class RegisteredObjectPersister : IReadPersister<RegisteredObjectReference>
    {

        public Task<ITransferResult> Exists<TValue>(IReferencable<RegisteredObjectReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<RegisteredObjectReference> referencable) => throw new NotImplementedException();
    }
}
#endif
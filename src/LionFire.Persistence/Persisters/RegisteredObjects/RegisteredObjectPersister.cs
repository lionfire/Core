using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.RegisteredObjects
{
    public class RegisteredObjectPersister : IReadPersister<RegisteredObjectReference>
    {

        public Task<IPersistenceResult> Exists<TValue>(IReferencable<RegisteredObjectReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<RegisteredObjectReference> referencable) => throw new NotImplementedException();
    }
}

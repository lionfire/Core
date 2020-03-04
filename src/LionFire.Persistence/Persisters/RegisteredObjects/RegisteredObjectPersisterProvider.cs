using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;

namespace LionFire.Persistence.RegisteredObjects
{
    public class RegisteredObjectPersisterProvider : IPersisterProvider<RegisteredObjectReference>
    {

        public bool HasDefaultPersister => throw new NotImplementedException();

        public IPersister<RegisteredObjectReference> GetPersister(string name = null) => throw new NotImplementedException();
    }
}

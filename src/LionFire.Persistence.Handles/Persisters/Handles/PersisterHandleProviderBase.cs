using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Persistence.Persisters
{
    // TODO Maybe: option to specify whether a shared handle is desired.

    public class PersisterHandleProviderBase<TReference>
        where TReference : IReference
    {
        protected IPersister<TReference> persister;

        public PersisterHandleProviderBase(IPersisterProvider<TReference> persisterProvider
            //, IPersisterProvider<ProviderVosReference> providerFilePersisterProvider
            )
        {
            persister = persisterProvider.GetPersister();
            //this.providerFilePersisterProvider = providerFilePersisterProvider;
        }

        public virtual IReadHandle<T> GetReadHandle<T>(TReference reference)
            => new PersisterReadHandle<TReference, T, IPersister<TReference>>(persister, reference);

        public virtual IReadWriteHandle<T> GetReadWriteHandle<T>(TReference reference)
            => new PersisterReadWriteHandle<TReference, T, IPersister<TReference>>(persister, reference);

        //public IWriteHandle<T> GetWriteHandle<T>(TReference reference)
                   //=> new PersisterWriteHandle<TReference, T, IPersister<TReference>>(persister, reference);

    }
}

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
            //, IPersisterProvider<ProviderVobReference> providerFilePersisterProvider
            )
        {
            persister = persisterProvider.GetPersister();
            //this.providerFilePersisterProvider = providerFilePersisterProvider;
        }

        public virtual IReadHandle<TValue> GetReadHandle<TValue>(IReference<TValue> reference, TValue preresolvedValue = default)
            => new PersisterReadHandle<TReference, TValue, IPersister<TReference>>(persister, reference, preresolvedValue);
        
        public virtual IReadHandle<TValue> GetReadHandle<TValue>(TReference reference, TValue preresolvedValue = default)
            => new PersisterReadHandle<TReference, TValue, IPersister<TReference>>(persister, (IReference<TValue>)reference, preresolvedValue);

        public virtual IWriteHandle<TValue> GetWriteHandle<TValue>(IReference<TValue> reference, TValue preresolvedValue = default)
            => new PersisterWriteHandle<TReference, TValue, IPersister<TReference>>(persister, reference, preresolvedValue);

        public virtual IWriteHandle<TValue> GetWriteHandle<TValue>(TReference reference, TValue preresolvedValue = default)
            => new PersisterWriteHandle<TReference, TValue, IPersister<TReference>>(persister, (IReference<TValue>)(object)reference, preresolvedValue); // HARDCAST

        public virtual IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(IReference<TValue> reference, TValue preresolvedValue = default)
            => new PersisterReadWriteHandle<TReference, TValue, IPersister<TReference>>(persister, reference, preresolvedValue);

        public virtual IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TReference reference, TValue preresolvedValue = default)
            => new PersisterReadWriteHandle<TReference, TValue, IPersister<TReference>>(persister, (IReference<TValue>)reference, preresolvedValue);

        //public virtual IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(TReference reference, TValue preresolvedValue = default)
        //    => new PersisterReadWriteHandle<TReference, TValue, IPersister<TReference>>(persister, reference, preresolvedValue);

        //public virtual IWriteHandle<TValue> GetWriteHandle<TValue>(TReference reference, TValue preresolvedValue = default)
        //    => new PersisterWriteHandle<TReference, TValue, IPersister<TReference>>(persister, reference, preresolvedValue);

    }
}

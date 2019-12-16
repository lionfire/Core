using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Handles
{
    public class HandleProviderBase<TReference>
        where TReference : IReference
    {
        protected IPersister<TReference> persister;

    }

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

        public IReadHandle<T> GetReadHandle<T>(TReference reference)
            => new PersisterReadWriteHandle<TReference, T, IPersister<TReference>>(persister, reference);

        public IReadWriteHandle<T> GetReadWriteHandle<T>(TReference reference)
            => new PersisterReadWriteHandle<TReference, T, IPersister<TReference>>(persister, reference);

    }

    public class VosHandleProvider : PersisterHandleProviderBase<VosReference>
        , IReadHandleProvider<VosReference>
        , IReadWriteHandleProvider<VosReference>
        , IReadHandleProvider // REVIEW
    //, IReadHandleProvider<ProviderVosReference>
    {
        //IPersisterProvider<ProviderVosReference> providerFilePersisterProvider;

        public VosHandleProvider(
            IPersisterProvider<VosReference> persisterProvider
            //, IPersisterProvider<ProviderVosReference> providerFilePersisterProvider
            ) : base(persisterProvider)
        {
            //this.providerFilePersisterProvider = providerFilePersisterProvider;
        }

        IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference) => GetReadHandle<T>((VosReference)reference);  // REVIEW

        //public IReadHandle<T> GetReadHandle<T>(ProviderVosReference reference)
        //    => new PersisterReadWriteHandle<ProviderVosReference, T, IPersister<ProviderVosReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

        //#warning TODO: ReadWrite handle

    }
}

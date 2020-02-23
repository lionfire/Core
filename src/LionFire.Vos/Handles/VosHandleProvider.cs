using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Vos.Handles
{
    public class VosHandleProvider : PersisterHandleProviderBase<VosReference>
        , IReadHandleProvider<VosReference>
        , IReadHandleProvider // REVIEW
        , IReadWriteHandleProvider<VosReference>
        , IReadWriteHandleProvider // REVIEW
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


        public override IReadHandle<T> GetReadHandle<T>(VosReference reference) 
            => reference.ToVob().GetReadHandle<T>();

        IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference) => GetReadHandle<T>((VosReference)reference);  // REVIEW
        IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference) => GetReadWriteHandle<T>((VosReference)reference);  // REVIEW

        //public IReadHandle<T> GetReadHandle<T>(ProviderVosReference reference)
        //    => new PersisterReadWriteHandle<ProviderVosReference, T, IPersister<ProviderVosReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

        //#warning TODO: ReadWrite handle

    }
}

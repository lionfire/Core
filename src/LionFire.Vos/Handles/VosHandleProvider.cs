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
        , IWriteHandleProvider<VosReference>
        , IWriteHandleProvider // REVIEW
    //, IReadHandleProvider<ProviderVosReference>
    {

        public VosHandleProvider(IPersisterProvider<VosReference> persisterProvider) : base(persisterProvider)
        {
        }


        public override IReadHandle<T> GetReadHandle<T>(VosReference reference)
            => reference.ToVob().GetReadHandle<T>();
        public override IReadWriteHandle<T> GetReadWriteHandle<T>(VosReference reference)
                  => reference.ToVob().GetReadWriteHandle<T>();
        public override IWriteHandle<T> GetWriteHandle<T>(VosReference reference)
           => reference.ToVob().GetWriteHandle<T>();
        //IWriteHandle<T> GetWriteHandle<T>(VosReference reference) 
        //=> reference.ToVob().GetWriteHandle<T>();

        IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference) => GetReadHandle<T>((VosReference)reference);  // REVIEW
        IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference) => GetReadWriteHandle<T>((VosReference)reference);  // REVIEW
        IWriteHandle<T> IWriteHandleProvider.GetWriteHandle<T>(IReference reference) => GetWriteHandle<T>((VosReference)reference); // REVIEW


        //public IReadHandle<T> GetReadHandle<T>(ProviderVosReference reference)
        //    => new PersisterReadWriteHandle<ProviderVosReference, T, IPersister<ProviderVosReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

        //#warning TODO: ReadWrite handle

    }
}

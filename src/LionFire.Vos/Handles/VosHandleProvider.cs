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


        public override IReadHandle<T> GetReadHandle<T>(VosReference reference, T preresolvedValue = default)
            => reference.GetVob().GetReadHandle<T>(preresolvedValue);
        public override IReadWriteHandle<T> GetReadWriteHandle<T>(VosReference reference, T preresolvedValue = default)
                  => reference.GetVob().GetReadWriteHandle<T>(preresolvedValue);
        public override IWriteHandle<T> GetWriteHandle<T>(VosReference reference, T prestagedValue = default)
           => reference.GetVob().GetWriteHandle<T>(prestagedValue);
        //IWriteHandle<T> GetWriteHandle<T>(VosReference reference) 
        //=> reference.ToVob().GetWriteHandle<T>();

        IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference, T preresolvedValue) => GetReadHandle<T>((VosReference)reference, preresolvedValue);  // REVIEW
        IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => GetReadWriteHandle<T>((VosReference)reference, preresolvedValue);  // REVIEW
        IWriteHandle<T> IWriteHandleProvider.GetWriteHandle<T>(IReference reference, T prestagedValue) => GetWriteHandle<T>((VosReference)reference, prestagedValue); // REVIEW


        //public IReadHandle<T> GetReadHandle<T>(ProviderVosReference reference)
        //    => new PersisterReadWriteHandle<ProviderVosReference, T, IPersister<ProviderVosReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

        //#warning TODO: ReadWrite handle

    }
}

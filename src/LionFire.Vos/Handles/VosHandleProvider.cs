using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Vos.Handles
{
    public class VosHandleProvider : PersisterHandleProviderBase<VobReference>
        , IReadHandleProvider<VobReference>
        , IReadHandleProvider // REVIEW
        , IReadWriteHandleProvider<VobReference>
        , IReadWriteHandleProvider // REVIEW
        , IWriteHandleProvider<VobReference>
        , IWriteHandleProvider // REVIEW
    //, IReadHandleProvider<ProviderVobReference>
    {

        public VosHandleProvider(IPersisterProvider<VobReference> persisterProvider) : base(persisterProvider)
        {
        }


        public override IReadHandle<T> GetReadHandle<T>(VobReference reference, T preresolvedValue = default)
            => reference.GetVob().GetReadHandle<T>(preresolvedValue);
        public override IReadWriteHandle<T> GetReadWriteHandle<T>(VobReference reference, T preresolvedValue = default)
                  => reference.GetVob().GetReadWriteHandle<T>(preresolvedValue);
        public override IWriteHandle<T> GetWriteHandle<T>(VobReference reference, T prestagedValue = default)
           => reference.GetVob().GetWriteHandle<T>(prestagedValue);
        //IWriteHandle<T> GetWriteHandle<T>(VobReference reference) 
        //=> reference.ToVob().GetWriteHandle<T>();

        IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference, T preresolvedValue) => GetReadHandle<T>((VobReference)reference, preresolvedValue);  // REVIEW
        IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => GetReadWriteHandle<T>((VobReference)reference, preresolvedValue);  // REVIEW
        IWriteHandle<T> IWriteHandleProvider.GetWriteHandle<T>(IReference reference, T prestagedValue) => GetWriteHandle<T>((VobReference)reference, prestagedValue); // REVIEW


        //public IReadHandle<T> GetReadHandle<T>(ProviderVobReference reference)
        //    => new PersisterReadWriteHandle<ProviderVobReference, T, IPersister<ProviderVobReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

        //#warning TODO: ReadWrite handle

    }
}

using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Vos.Handles
{
    public class VosHandleProvider : PersisterHandleProviderBase<IVobReference>
        , IReadHandleProvider<IVobReference>
        , IReadHandleProvider // REVIEW
        , IReadWriteHandleProvider<IVobReference>
        , IReadWriteHandleProvider // REVIEW
        , IWriteHandleProvider<IVobReference>
        , IWriteHandleProvider // REVIEW
    //, IReadHandleProvider<ProviderVobReference>
    {

        public VosHandleProvider(IPersisterProvider<IVobReference> persisterProvider) : base(persisterProvider)
        {
        }


        public override IReadHandle<T> GetReadHandle<T>(IVobReference reference, T preresolvedValue = default)
            => reference.GetVob().GetReadHandle<T>(preresolvedValue);
        public override IReadWriteHandle<T> GetReadWriteHandle<T>(IVobReference reference, T preresolvedValue = default)
                  => reference.GetVob().GetReadWriteHandle<T>(preresolvedValue);
        public override IWriteHandle<T> GetWriteHandle<T>(IVobReference reference, T prestagedValue = default)
           => reference.GetVob().GetWriteHandle<T>(prestagedValue);
        //IWriteHandle<T> GetWriteHandle<T>(IVobReference reference) 
        //=> reference.ToVob().GetWriteHandle<T>();

        IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference, T preresolvedValue) => GetReadHandle<T>((IVobReference)reference, preresolvedValue);  // REVIEW
        IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => GetReadWriteHandle<T>((IVobReference)reference, preresolvedValue);  // REVIEW
        IWriteHandle<T> IWriteHandleProvider.GetWriteHandle<T>(IReference reference, T prestagedValue) => GetWriteHandle<T>((IVobReference)reference, prestagedValue); // REVIEW


        //public IReadHandle<T> GetReadHandle<T>(ProviderVobReference reference)
        //    => new PersisterReadWriteHandle<ProviderVobReference, T, IPersister<ProviderVobReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

        //#warning TODO: ReadWrite handle

    }
}

using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Vos.Handles;

public class VosHandleProvider : PersisterHandleProviderBase<IVobReference>
    , IPreresolvableReadHandleProvider<IVobReference>
    , IReadWriteHandleProvider<IVobReference>
    , IReadWriteHandleProvider // REVIEW - should IReadWriteHandleProvider<IVobReference> inherit from this?
    , IWriteHandleProvider<IVobReference>
    , IWriteHandleProvider // REVIEW
//, IReadHandleProvider<ProviderVobReference>
{

    public VosHandleProvider(IPersisterProvider<IVobReference> persisterProvider) : base(persisterProvider)
    {
    }

    #region R

    public override IReadHandle<T> GetReadHandle<T>(IVobReference reference)
        => reference.GetVob().GetReadHandle<T>();
    public override IReadHandle<T> GetReadHandlePreresolved<T>(IVobReference reference, T preresolvedValue = default)
        => reference.GetVob().GetReadHandle<T>(preresolvedValue);

    IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference) => GetReadHandle<T>((IVobReference)reference.Innermost());  // REVIEW
    IReadHandle<T> IPreresolvableReadHandleProvider.GetReadHandlePreresolved<T>(IReference reference, T preresolvedValue) => GetReadHandlePreresolved<T>((IVobReference)reference, preresolvedValue);  // REVIEW

    #endregion

    #region RW

    public override IReadWriteHandle<T> GetReadWriteHandle<T>(IVobReference reference, T preresolvedValue = default)
              => reference.GetVob().GetReadWriteHandle<T>(preresolvedValue);
    
    IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => GetReadWriteHandle<T>((IVobReference)reference, preresolvedValue);  // REVIEW

    #endregion

    #region W

    public override IWriteHandle<T> GetWriteHandle<T>(IVobReference reference, T prestagedValue = default)
       => reference.GetVob().GetWriteHandle<T>(prestagedValue);

    IWriteHandle<T> IWriteHandleProvider.GetWriteHandle<T>(IReference reference, T prestagedValue) => GetWriteHandle<T>((IVobReference)reference, prestagedValue); // REVIEW

    //IWriteHandle<T> GetWriteHandle<T>(IVobReference reference) 
    //=> reference.ToVob().GetWriteHandle<T>();

    #endregion

    //public IReadHandle<T> GetReadHandle<T>(ProviderVobReference reference)
    //    => new PersisterReadWriteHandle<ProviderVobReference, T, IPersister<ProviderVobReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);
}

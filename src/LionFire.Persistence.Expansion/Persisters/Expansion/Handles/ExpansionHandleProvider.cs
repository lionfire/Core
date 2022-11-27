using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;

namespace LionFire.Persisters.Expansion;

public class ExpansionHandleProvider : HandleProviderBase<IExpansionReference>
    // PersisterHandleProviderBase<IExpansionReference>
     , IReadHandleProvider<IExpansionReference>
    //, IReadHandleProvider // REVIEW
    //, IReadWriteHandleProvider<IExpansionReference>
    //, IReadWriteHandleProvider // REVIEW
    //, IWriteHandleProvider<IExpansionReference>
    //, IWriteHandleProvider // REVIEW
//, IReadHandleProvider<ProviderVobReference>
{
    #region Dependencies

    public IServiceProvider ServiceProvider { get; }

    #endregion

    #region Lifecycle

    public ExpansionHandleProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    #endregion

    public override IReadHandle<TValue> GetReadHandle<TValue>(IExpansionReference reference, TValue preresolvedValue = default)
        //where TValue : default
        => new ExpansionReadHandle<TValue>((ExpansionReference<TValue>)reference, preresolvedValue);

    //public override IReadHandle<TValue> GetReadHandle<TValue>(ExpansionReference reference, TValue preresolvedValue = default)
    //{
    //    throw new NotImplementedException();
    //}

    //public override IReadHandle<TValue> GetReadHandle<TValue>(ExpansionReference reference, TValue preresolvedValue = default)
    //{
    //    throw new NotImplementedException();
    //}

    //public IReadHandle<T> GetReadHandle<T>(IExpansionReference reference, T? preresolvedValue = default)
    //{
    //    throw new NotImplementedException();
    //}

    //public override IReadHandle<TValue> GetReadHandle<TValue>(ExpansionReference reference, TValue? preresolvedValue = default) 
    //    //where TValue : default
    //{
    //    throw new NotImplementedException();
    //}


    //public override IReadHandle<T> GetReadHandle<T>(IExpansionReference reference, T preresolvedValue = default)
    //    => reference.GetVob().GetReadHandle<T>(preresolvedValue);
    //public override IReadWriteHandle<T> GetReadWriteHandle<T>(IExpansionReference reference, T preresolvedValue = default)
    //          => reference.GetVob().GetReadWriteHandle<T>(preresolvedValue);
    //public override IWriteHandle<T> GetWriteHandle<T>(IExpansionReference reference, T prestagedValue = default)
    //   => reference.GetVob().GetWriteHandle<T>(prestagedValue);
    ////IWriteHandle<T> GetWriteHandle<T>(IExpansionReference reference) 
    ////=> reference.ToVob().GetWriteHandle<T>();

    //IReadHandle<T> IReadHandleProvider.GetReadHandle<T>(IReference reference, T preresolvedValue) => GetReadHandle<T>((IExpansionReference)reference, preresolvedValue);  // REVIEW
    //IReadWriteHandle<T> IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => GetReadWriteHandle<T>((IExpansionReference)reference, preresolvedValue);  // REVIEW
    //IWriteHandle<T> IWriteHandleProvider.GetWriteHandle<T>(IReference reference, T prestagedValue) => GetWriteHandle<T>((IExpansionReference)reference, prestagedValue); // REVIEW


    //public IReadHandle<T> GetReadHandle<T>(ProviderVobReference reference)
    //    => new PersisterReadWriteHandle<ProviderVobReference, T, IPersister<ProviderVobReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

    //#warning TODO: ReadWrite handle

}

using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Persisters.Expanders;

public class ExpansionHandleProvider : HandleProviderBase<IExpansionReference>
     // PersisterHandleProviderBase<IExpansionReference>
     , IReadHandleProvider
     , IReadHandleProvider<IExpansionReference>
//, IReadWriteHandleProvider<IExpansionReference>
//, IReadWriteHandleProvider // REVIEW
//, IWriteHandleProvider<IExpansionReference>
//, IWriteHandleProvider // REVIEW
//, IReadHandleProvider<ProviderVobReference>
{
    #region Dependencies

    public IServiceProvider ServiceProvider { get; }
    public IExpanderProvider ExpanderProvider { get; }
    public IReferenceProviderService ReferenceProviderService { get; }
    public IReferenceToHandleService ReferenceToHandleService { get; }
    //public IReadHandleProvider ReadHandleProvider { get; }

    #endregion

    #region Lifecycle

    public ExpansionHandleProvider(IServiceProvider serviceProvider, IExpanderProvider expanderProvider, IReferenceProviderService referenceProviderService
        , IReferenceToHandleService referenceToHandleService
        //, IReadHandleProviderService readHandleProvider // TODO
        )
    {
        ServiceProvider = serviceProvider;
        ExpanderProvider = expanderProvider;
        ReferenceProviderService = referenceProviderService;
        ReferenceToHandleService = referenceToHandleService;
        //ReadHandleProvider = readHandleProvider;
    }

    #endregion

    public override IReadHandle<TValue> GetReadHandle<TValue>(IExpansionReference reference)
    //where TValue : default
    {
        var sourceReference = ReferenceProviderService.GetReference(reference.SourceUri);

        return new ExpanderReadHandle<TValue>(ExpanderProvider, sourceReference, (ExpansionReference<TValue>)reference);

        //throw new Exception("NEXT: Resolve reference.SourceUri to source IReadHandle<TValue> and use it to construct ExpansionReadHandle");
        // new ExpansionReadHandle<TValue>((ExpansionReference<TValue>)reference);
        //if (SourceReadHandle == null)
        //{
        //    var result = ReferenceProviderService.TryGetReference<IReference>(Reference.SourceKey);
        //    if (result.error != null)
        //    {
        //        throw new Exception($"Error getting reference for {nameof(Reference.SourceKey)}: {result.error}");
        //    }
        //    else
        //    {
        //        SourceReference = result.result;
        //    }
        //}
    }

    public static ExpansionReference<TValue>? TryCoerceReferenceType<TValue>(IReference reference)
    {
        var result = reference as ExpansionReference<TValue>;
        if (result != null) return result;

        var nativeReference = reference as IExpansionReference;
        if (nativeReference == null) return null;

        return new ExpansionReference<TValue>(nativeReference.SourceUri, nativeReference.Path);
    }
    public IReadHandle<TValue>? GetReadHandle<TValue>(IReference reference)
    {
        var nativeReference = TryCoerceReferenceType<TValue>(reference);
        if (nativeReference == null) return null;

        var sourceReference = ReferenceProviderService.GetReference(nativeReference.SourceUri);
        return new ExpanderReadHandle<TValue>(ExpanderProvider, sourceReference, nativeReference);
    }

    //public override IReadHandle<TValue> GetReadHandle<TValue>(ExpansionReference reference, TValue preresolvedValue = default)
    //{
    //    throw new NotImplementedException();
    //}

    //public override IReadHandle<TValue> GetReadHandle<TValue>(ExpansionReference reference, TValue preresolvedValue = default)
    //{
    //    throw new NotImplementedException();
    //}

    //public IReadHandle<TValue> GetReadHandle<TValue>(IExpansionReference reference, TValue? preresolvedValue = default)
    //{
    //    throw new NotImplementedException();
    //}

    //public override IReadHandle<TValue> GetReadHandle<TValue>(ExpansionReference reference, TValue? preresolvedValue = default) 
    //    //where TValue : default
    //{
    //    throw new NotImplementedException();
    //}


    //public override IReadHandle<TValue> GetReadHandle<TValue>(IExpansionReference reference, TValue preresolvedValue = default)
    //    => reference.GetVob().GetReadHandle<TValue>(preresolvedValue);
    //public override IReadWriteHandle<TValue> GetReadWriteHandle<TValue>(IExpansionReference reference, TValue preresolvedValue = default)
    //          => reference.GetVob().GetReadWriteHandle<TValue>(preresolvedValue);
    //public override IWriteHandle<TValue> GetWriteHandle<TValue>(IExpansionReference reference, TValue prestagedValue = default)
    //   => reference.GetVob().GetWriteHandle<TValue>(prestagedValue);
    ////IWriteHandle<TValue> GetWriteHandle<TValue>(IExpansionReference reference) 
    ////=> reference.ToVob().GetWriteHandle<TValue>();

    //IReadHandle<TValue> IReadHandleProvider.GetReadHandle<TValue>(IReference reference, TValue preresolvedValue) => GetReadHandle<TValue>((IExpansionReference)reference, preresolvedValue);  // REVIEW
    //IReadWriteHandle<TValue> IReadWriteHandleProvider.GetReadWriteHandle<TValue>(IReference reference, TValue preresolvedValue) => GetReadWriteHandle<TValue>((IExpansionReference)reference, preresolvedValue);  // REVIEW
    //IWriteHandle<TValue> IWriteHandleProvider.GetWriteHandle<TValue>(IReference reference, TValue prestagedValue) => GetWriteHandle<TValue>((IExpansionReference)reference, prestagedValue); // REVIEW


    //public IReadHandle<TValue> GetReadHandle<TValue>(ProviderVobReference reference)
    //    => new PersisterReadWriteHandle<ProviderVobReference, TValue, IPersister<ProviderVobReference>>(providerFilePersisterProvider.GetPersister(reference.Persister), reference);

    //#warning TODO: ReadWrite handle

}

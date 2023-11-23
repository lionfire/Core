using System;

namespace LionFire.Referencing;

public static class IReferenceProviderServiceX
{

    public static (IReference reference, string error) TryGetReference(this IReferenceProviderService referenceProviderService, string uri) => referenceProviderService.TryGetReference<IReference>(uri);

    //public static IReference GetReference(this IReferenceProviderService referenceProviderService, string uri) => referenceProviderService.GetReference<IReference>(uri);


    #region GetReference

    // Generic
    public static TReference GetReference<TReference>(this IReferenceProvider referenceProvider, string uri)
        where TReference : IReference
    {
        var result = referenceProvider.TryGetReference<TReference>(uri);
        if(result.result != null) { return result.result; }
        throw new Exception($"Failed to get Reference for uri: '{uri}'.  Reason: {result.error ?? "(none available)"}");
    }

    // Non-generic
    public static IReference GetReference(this IReferenceProvider referenceProvider, string uri)
    {
        var result = referenceProvider.TryGetReference<IReference>(uri);
        if (result.result != null) { return result.result; }
        throw new Exception($"Failed to get Reference for uri: '{uri}'.  Reason: {result.error ?? "(none available)"}");
    }

    //public static IReference GetReference(this IReferenceProviderService referenceProviderService, string uri)
    //    => ((IReferenceProvider)referenceProviderService).GetReference<TReference>(uri).result ?? throw new NotFoundException();

    #endregion

}

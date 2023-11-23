using LionFire.Dependencies;
using LionFire.Persistence.Handles;
using System;

namespace LionFire.Referencing.Ex;

// partial: CollectionHandle
public static partial class ReferenceToHandleProviderExtensions 
{
    //#error NEXT: Debug _SimpleMount.Pass, then add IServiceProvider parameters to other methods here

    #region ICollectionHandleProvider

    public static ICollectionHandleProvider GetCollectionHandleProvider(this IReference reference)
        => ServiceLocator.Get<IReferenceToHandleService>().GetCollectionHandleProvider(reference);
    public static ICollectionHandleProvider ToCollectionHandleProvider(this IReference reference)
        => GetCollectionHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(ICollectionHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

    #endregion
}

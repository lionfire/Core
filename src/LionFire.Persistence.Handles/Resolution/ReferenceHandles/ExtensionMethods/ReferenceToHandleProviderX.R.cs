using LionFire.Dependencies;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing.Ex;

// partial: ReadHandle
public static partial class ReferenceToProviderExtensions 
{
    // Generic
    public static IReadHandleProvider<TReference> TryGetReadHandleProvider<TReference>(this TReference reference, IServiceProvider serviceProvider = null)
        where TReference : IReference
        => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetReadHandleProvider(reference);

    // Non-generic
    public static IReadHandleProvider TryGetReadHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
        => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetReadHandleProvider(reference);

    #region ...or throw

    public static IReadHandleProvider GetReadHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
        => TryGetReadHandleProvider(reference, serviceProvider)
              ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

    #endregion

    #region IReadHandleCreator

    public static IReadHandleCreator GetReadHandleCreator(this IReference reference)
        => ServiceLocator.Get<IReferenceToHandleService>().GetReadHandleCreator(reference);

    public static IReadHandleCreator<TReference> GetReadHandleCreator<TReference>(this TReference reference)
        where TReference : IReference
        => ServiceLocator.Get<IReferenceToHandleService>().GetReadHandleCreator<TReference>(reference);

    public static IReadHandleCreator ToReadHandleCreator(this IReference reference)
        => GetReadHandleCreator(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadHandleCreator)} could be found for reference of type {reference?.GetType().FullName}");

    #endregion
}



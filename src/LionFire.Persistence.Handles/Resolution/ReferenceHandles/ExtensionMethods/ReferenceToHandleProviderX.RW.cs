using LionFire.Dependencies;
using LionFire.Persistence.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing.Ex;

// partial: ReadWriteHandle
public static partial class ReferenceToHandleProviderExtensions 
{
    #region IReadWriteHandleProvider

    public static IReadWriteHandleProvider TryGetReadWriteHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
        => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetReadWriteHandleProvider(reference);

    public static IReadWriteHandleProvider<TReference> TryGetReadWriteHandleProvider<TReference>(this TReference reference, IServiceProvider serviceProvider = null)
                where TReference : IReference
         => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetReadWriteHandleProvider<TReference>(reference);

    public static IReadWriteHandleProvider GetReadWriteHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
        => TryGetReadWriteHandleProvider(reference, serviceProvider) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadWriteHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

    #endregion

    #region IReadWriteHandleCreator

    public static IReadWriteHandleCreator GetReadWriteHandleCreator(this IReference reference)
        => ServiceLocator.Get<IReferenceToHandleService>().GetReadWriteHandleCreator(reference);

    public static IReadWriteHandleCreator<TReference> GetReadWriteHandleCreator<TReference>(this TReference reference)
                where TReference : IReference
         => ServiceLocator.Get<IReferenceToHandleService>().GetReadWriteHandleCreator<TReference>(reference);

    public static IReadWriteHandleCreator ToReadWriteHandleCreator(this IReference reference)
        => GetReadWriteHandleCreator(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadWriteHandleCreator)} could be found for reference of type {reference?.GetType().FullName}");

    #endregion
}
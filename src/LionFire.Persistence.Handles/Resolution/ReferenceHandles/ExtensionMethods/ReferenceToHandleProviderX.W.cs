using LionFire.Dependencies;
using LionFire.Persistence.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Referencing.Ex;

// partial: WriteHandle
public static partial class ReferenceToHandleProviderExtensions 
{

    #region IWriteOnlyHandleProvider

    public static IWriteHandleProvider TryGetWriteHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
        => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetWriteHandleProvider(reference);

    public static IWriteHandleProvider<TReference> TryGetWriteHandleProvider<TReference>(this TReference reference, IServiceProvider serviceProvider = null)
        where TReference : IReference
        => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetWriteHandleProvider<TReference>(reference);

    public static IWriteHandleProvider GetWriteHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
        => TryGetWriteHandleProvider(reference, serviceProvider) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IWriteHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

    #endregion

}

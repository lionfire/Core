using LionFire.DependencyInjection;
using LionFire.Persistence.Handles;

namespace LionFire.Referencing.Ex
{
    public static class ReferenceToHandleProviderExtensions
    {
        #region IReadHandleProvider

        public static IReadHandleProvider GetReadHandleProvider(this IReference reference)
            => DependencyContext.Current.GetServiceOrSingleton<IReferenceToHandleService>().GetReadHandleProvider(reference);

        public static IReadHandleProvider ToReadHandleProvider(this IReference reference) 
            => GetReadHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadHandleProvider)}could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region IHandleProvider

        public static IHandleProvider GetHandleProvider(this IReference reference)
            => DependencyContext.Current.GetServiceOrSingleton<IReferenceToHandleService>().GetHandleProvider(reference);
        public static IHandleProvider ToHandleProvider(this IReference reference) 
            => GetHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region ICollectionHandleProvider

        public static ICollectionHandleProvider GetCollectionHandleProvider(this IReference reference)
            => DependencyContext.Current.GetServiceOrSingleton<IReferenceToHandleService>().GetCollectionHandleProvider(reference);
        public static ICollectionHandleProvider ToCollectionHandleProvider(this IReference reference)
            => GetCollectionHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(ICollectionHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion
    }
}

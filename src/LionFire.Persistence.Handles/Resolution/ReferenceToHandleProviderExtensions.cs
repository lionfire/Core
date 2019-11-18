using LionFire.Dependencies;
using LionFire.Persistence.Handles;

namespace LionFire.Referencing.Ex
{
    public static class ReferenceToHandleProviderExtensions
    {
        #region IReadHandleProvider

        public static IReadHandleProvider GetReadHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadHandleProvider(reference);

        public static IReadHandleProvider ToReadHandleProvider(this IReference reference) 
            => GetReadHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadHandleProvider)}could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region IWriteOnlyHandleProvider

        public static IWriteHandleProvider GetWriteHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetWriteHandleProvider(reference);

        public static IWriteHandleProvider ToWriteHandleProvider(this IReference reference)
            => GetWriteHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IWriteHandleProvider)}could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region IHandleProvider

        public static IReadWriteHandleProvider GetReadWriteHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadWriteHandleProvider(reference);
        public static IReadWriteHandleProvider ToReadWriteHandleProvider(this IReference reference) 
            => GetReadWriteHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadWriteHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region ICollectionHandleProvider

        public static ICollectionHandleProvider GetCollectionHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetCollectionHandleProvider(reference);
        public static ICollectionHandleProvider ToCollectionHandleProvider(this IReference reference)
            => GetCollectionHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(ICollectionHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion
    }
}

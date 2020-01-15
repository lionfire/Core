using LionFire.Dependencies;
using LionFire.Persistence.Handles;

namespace LionFire.Referencing.Ex
{
    public static class ReferenceToHandleProviderExtensions
    {
        #region IReadHandleProvider

        public static IReadHandleProvider GetReadHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadHandleProvider(reference);

        public static IReadHandleProvider<TReference> GetReadHandleProvider<TReference>(this TReference reference)
            where TReference: IReference
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadHandleProvider<TReference>(reference);

        public static IReadHandleProvider ToReadHandleProvider(this IReference reference) 
            => GetReadHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region IReadHandleCreator

        public static IReadHandleCreator GetReadHandleCreator(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadHandleCreator(reference);

        public static IReadHandleCreator<TReference> GetReadHandleCreator<TReference>(this TReference reference)
            where TReference : IReference
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadHandleCreator<TReference>(reference);

        public static IReadHandleCreator ToReadHandleCreator(this IReference reference)
            => GetReadHandleCreator(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadHandleCreator)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion
        

        #region IReadWriteHandleProvider

        public static IReadWriteHandleProvider GetReadWriteHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadWriteHandleProvider(reference);

        public static IReadWriteHandleProvider<TReference> GetReadWriteHandleProvider<TReference>(this TReference reference)
                    where TReference : IReference
             => DependencyLocator.Get<IReferenceToHandleService>().GetReadWriteHandleProvider<TReference>(reference);

        public static IReadWriteHandleProvider ToReadWriteHandleProvider(this IReference reference) 
            => GetReadWriteHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadWriteHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region IReadWriteHandleCreator

        public static IReadWriteHandleCreator GetReadWriteHandleCreator(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetReadWriteHandleCreator(reference);

        public static IReadWriteHandleCreator<TReference> GetReadWriteHandleCreator<TReference>(this TReference reference)
                    where TReference : IReference
             => DependencyLocator.Get<IReferenceToHandleService>().GetReadWriteHandleCreator<TReference>(reference);

        public static IReadWriteHandleCreator ToReadWriteHandleCreator(this IReference reference)
            => GetReadWriteHandleCreator(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IReadWriteHandleCreator)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region IWriteOnlyHandleProvider

        public static IWriteHandleProvider GetWriteHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetWriteHandleProvider(reference);

        public static IWriteHandleProvider<TReference> GetWriteHandleProvider<TReference>(this TReference reference)
            where TReference : IReference
            => DependencyLocator.Get<IReferenceToHandleService>().GetWriteHandleProvider<TReference>(reference);

        public static IWriteHandleProvider ToWriteHandleProvider(this IReference reference)
            => GetWriteHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IWriteHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion
        
        #region ICollectionHandleProvider

        public static ICollectionHandleProvider GetCollectionHandleProvider(this IReference reference)
            => DependencyLocator.Get<IReferenceToHandleService>().GetCollectionHandleProvider(reference);
        public static ICollectionHandleProvider ToCollectionHandleProvider(this IReference reference)
            => GetCollectionHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(ICollectionHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion
    }
}

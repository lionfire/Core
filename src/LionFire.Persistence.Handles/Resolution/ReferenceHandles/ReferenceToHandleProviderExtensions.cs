using LionFire.Dependencies;
using LionFire.Persistence.Handles;
using System;

namespace LionFire.Referencing.Ex
{

    public static class ReferenceToHandleProviderExtensions
    {
        #region IReadHandleProvider

        //#error NEXT: Debug _SimpleMount.Pass, then add IServiceProvider parameters to other methods here

        public static IReadHandleProvider TryGetReadHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
            => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetReadHandleProvider(reference);

        public static IReadHandleProvider<TReference> TryGetReadHandleProvider<TReference>(this TReference reference, IServiceProvider serviceProvider = null)
            where TReference : IReference
            => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetReadHandleProvider<TReference>(reference);

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

        #region IWriteOnlyHandleProvider

        public static IWriteHandleProvider TryGetWriteHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
            => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetWriteHandleProvider(reference);

        public static IWriteHandleProvider<TReference> TryGetWriteHandleProvider<TReference>(this TReference reference, IServiceProvider serviceProvider = null)
            where TReference : IReference
            => ServiceLocator.Get<IReferenceToHandleService>(serviceProvider).GetWriteHandleProvider<TReference>(reference);

        public static IWriteHandleProvider GetWriteHandleProvider(this IReference reference, IServiceProvider serviceProvider = null)
            => TryGetWriteHandleProvider(reference, serviceProvider) ?? throw new HasUnresolvedDependenciesException($"No {nameof(IWriteHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion

        #region ICollectionHandleProvider

        public static ICollectionHandleProvider GetCollectionHandleProvider(this IReference reference)
            => ServiceLocator.Get<IReferenceToHandleService>().GetCollectionHandleProvider(reference);
        public static ICollectionHandleProvider ToCollectionHandleProvider(this IReference reference)
            => GetCollectionHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No {nameof(ICollectionHandleProvider)} could be found for reference of type {reference?.GetType().FullName}");

        #endregion
    }
}

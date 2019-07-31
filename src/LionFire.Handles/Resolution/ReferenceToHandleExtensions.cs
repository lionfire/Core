using LionFire.DependencyInjection;
using LionFire.Referencing;
using LionFire.Referencing.Handles;

namespace LionFire.Referencing
{
    public static class ReferenceToHandleImplementationExtensions
    {

        public static IHandleProvider TryGetHandleProvider(this IReference reference)
            => DependencyContext.Current.GetServiceOrSingleton<IReferenceToHandleService>().GetHandleProvider(reference);
        public static IHandleProvider GetHandleProvider(this IReference reference) =>
            TryGetHandleProvider(reference) ?? throw new HasUnresolvedDependenciesException($"No IHandleProvider could be found for reference of type {reference?.GetType().FullName}");

        public static IReadHandleProvider GetReadHandleProvider(this IReference reference)
            => DependencyContext.Current.GetServiceOrSingleton<IReferenceToHandleService>().GetReadHandleProvider(reference);

        public static ICollectionHandleProvider GetCollectionHandleProvider(this IReference reference)
            => DependencyContext.Current.GetServiceOrSingleton<IReferenceToHandleService>().GetCollectionHandleProvider(reference);

        public static H<T> GetHandle<T>(this IReference reference) => reference.GetHandleProvider().GetHandle<T>(reference);
        public static RH<T> GetReadHandle<T>(this IReference reference) => reference.GetReadHandleProvider().GetReadHandle<T>(reference);

        public static C<T> GetCollectionHandle<T>(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<T>(reference);
        public static C<object> GetCollectionHandle(this IReference reference) => reference.GetCollectionHandleProvider().GetCollectionHandle<object>(reference);
    }
}

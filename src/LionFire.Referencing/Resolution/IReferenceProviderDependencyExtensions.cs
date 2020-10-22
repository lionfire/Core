using LionFire.Dependencies;

namespace LionFire.Referencing
{
    public static class IReferenceProviderDependencyExtensions
    {
        public static (IReference reference, string error) TryToReference(this string uri) 
            => DependencyContext.Current.GetService<IReferenceProviderService>().TryGetReference<IReference>(uri);
        public static TReference TryToReference<TReference>(this string uri) where TReference : IReference 
            => DependencyContext.Current.GetService<IReferenceProviderService>().TryGetReference<TReference>(uri).result;
        public static TReference ToReference<TReference>(this string uri) where TReference : IReference 
            => (DependencyContext.Current.GetService<IReferenceProviderService>() ?? throw new HasUnresolvedDependenciesException(nameof(IReferenceProviderService)))
            .GetReference<TReference>(uri) ?? throw new NotFoundException($"Failed to resolve reference for '{uri}'");
        public static IReference ToReference(this string uri)  => uri.ToReference<IReference>();
    }
}

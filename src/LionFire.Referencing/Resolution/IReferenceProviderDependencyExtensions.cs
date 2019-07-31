using LionFire.DependencyInjection;

namespace LionFire.Referencing
{
    public static class IReferenceProviderDependencyExtensions
    {
        public static IReference TryToReference(this string uri) =>
            DependencyContext.Current.GetService<IReferenceProviderService>().TryGetReference(uri);
        public static IReference ToReference(this string uri) =>
            (DependencyContext.Current.GetService<IReferenceProviderService>() ?? throw new HasUnresolvedDependenciesException(nameof(IReferenceProviderService)))
            .GetReference(uri) ?? throw new NotFoundException();
    }
}

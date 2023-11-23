using LionFire.Dependencies;

namespace LionFire.Referencing;

public static class IReferenceProviderDependencyExtensions
{
    public static (IReference reference, string error) TryToReference(this string uri) 
        => DependencyContext.Current.GetService<IReferenceProviderService>().TryGetReference<IReference>(uri);
    public static TReference TryToReference<TReference>(this string uri) where TReference : IReference 
        => DependencyContext.Current.GetService<IReferenceProviderService>().TryGetReference<TReference>(uri).result;

    public static TReference ToReferenceType<TReference>(this string uri) where TReference : IReference 
        => DependencyContext.Current.GetRequiredService<IReferenceProviderService>()
            .GetReference<TReference>(uri) 
            ?? throw new NotFoundException($"Failed to resolve reference for '{uri}'");

    // TODO
    //public static TReference ToReferenceType<TReference,TValue>(this string uri) where TReference : IReference<TValue>

    public static IReference ToReference(this string uri) => uri.ToReferenceType<IReference>();
    public static IReference<TValue> ToReference<TValue>(this string uri) => uri.ToReferenceType<IReference<TValue>>();

}

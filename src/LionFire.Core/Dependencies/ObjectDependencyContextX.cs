#nullable enable
using LionFire.Dependencies;

namespace LionFire.ExtensionMethods.Objects;

public static class ObjectServiceProviderX
{
    public static IServiceProvider? TryGetServiceProvider(this object obj, bool allowUnwrapDefault = false)
    {
        if (obj.TryUnwrapAs<IServiceProvider>(out var r, allowUnwrapDefault)) return r;
        return null;
    }

    public static IServiceProvider? GetAmbientServiceProvider(this object obj )
        => obj.TryGetServiceProvider(allowUnwrapDefault: false) ?? DependencyContext.Current?.ServiceProvider;
}

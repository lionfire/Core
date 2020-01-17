#nullable enable
using LionFire.Dependencies;

namespace LionFire.Vos
{
    public static class RootManagerExtensions
    {
        public static IVob? ToVob(this string vosPath) => DependencyContext.Current.GetServiceOrSingleton<RootManager>(createIfMissing: false)?.Get()?[vosPath];
        public static IVob? ToVob(this IVosReference vosReference) => DependencyContext.Current.GetServiceOrSingleton<RootManager>(createIfMissing: false)?.Get(vosReference.Persister)?[vosReference.PathChunks];
    }
}

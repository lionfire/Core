#nullable enable
using LionFire.Dependencies;

namespace LionFire.Vos
{
    public static class VosRootManagerExtensions
    {
        public static IVob? ToVob(this string vosPath) => DependencyContext.Current.GetServiceOrSingleton<VosRootManager>(createIfMissing: false)?.Get()?[vosPath];
        public static IVob? ToVob(this IVosReference vosReference) => DependencyContext.Current.GetServiceOrSingleton<VosRootManager>(createIfMissing: false)?.Get(vosReference.Persister)?[vosReference.PathChunks];
    }
}

using Microsoft.Extensions.DependencyInjection;
using LionFire.Vos;
using LionFire.Dependencies;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public static class VosMountServiceCollectionExtensions
    {
        public static IServiceCollection VosMount(this IServiceCollection services, string vosPath, IReference reference, MountOptions options = null)
           //=> services.TryAddEnumerableSingleton(new TMount(vosPath, reference, options));
           => services.Configure<VosOptions>(o => o.Mounts.Add(new TMount(vosPath, reference, options)));

    }
}

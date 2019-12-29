using Microsoft.Extensions.DependencyInjection;
using LionFire.Vos;
using LionFire.Dependencies;
using LionFire.Referencing;
using LionFire.Vos.Mounts;

namespace LionFire.Vos
{
    public static class VosMountServiceCollectionExtensions
    {
        /// <summary>
        /// Defaults to Read mount if no options specified
        /// </summary>
        public static IServiceCollection VosMount(this IServiceCollection services, string vosPath, IReference reference, IMountOptions options = null) 
            => services.Configure<VosOptions>(o => o.Mounts.Add(new TMount(vosPath, reference, options ?? MountOptions.DefaultRead)));


        /// <summary>
        /// Defaults to Read mount if no options specified
        /// </summary>
        /// <param name="services"></param>
        /// <param name="vosPath"></param>
        /// <param name="reference"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection VosMountRead(this IServiceCollection services, string vosPath, IReference reference)
           => VosMount(services, vosPath, reference, MountOptions.DefaultRead);
        public static IServiceCollection VosMountReadWrite(this IServiceCollection services, string vosPath, IReference reference)
            => VosMount(services, vosPath, reference, MountOptions.DefaultReadWrite);
        public static IServiceCollection VosMountWrite(this IServiceCollection services, string vosPath, IReference reference)
            => VosMount(services, vosPath, reference, MountOptions.DefaultWrite);

    }
}

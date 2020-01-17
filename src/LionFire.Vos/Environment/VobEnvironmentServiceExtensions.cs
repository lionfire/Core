using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public static class VobEnvironmentServiceExtensions
    {
        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, VosReference vob, string key, T value, bool addEnvironmentNodeAtVobIfMissing = true)
          => services.InitializeVob(vob, v =>
                   (addEnvironmentNodeAtVobIfMissing
                   ? v.GetOrAddOwn<VobEnvironment>()
                   : v.GetNextOrCreateAtRoot<VobEnvironment>())
                   [key] = value);

        /// <summary>
        /// Set environment on root Vob
        /// </summary>
        /// <param name="services"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, string key, T value)
            => services.VobEnvironment<T>("/", key, value, addEnvironmentNodeAtVobIfMissing: true);

    }
}

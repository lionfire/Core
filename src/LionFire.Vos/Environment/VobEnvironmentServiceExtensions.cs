using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public static class VobEnvironmentServiceExtensions
    {
        public static IServiceCollection VobEnvironment(this IServiceCollection services, string key, object value)
        {
            services.InitializeRootVob(v =>
            {
                v.GetOrAddOwn<VobEnvironment>()
                    .Add(key, value)
                    ;
            });
            return services;
        }
    }
}

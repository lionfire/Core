using LionFire.MultiTyping;
using LionFire.Vos;
using LionFire.Vos.Packages;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public static class VosPackageManagerServicesExtensions
    {
        public static IServiceCollection VosPackageManager(this IServiceCollection services, IVosReference vosReference, PackageManagerOptions options = null)
        {
            services.InitializeVob(vosReference, (serviceProvider, v) =>
             {
                 v.AddPackageManager(options);
             });
            return services;
        }
        public static IServiceCollection VosPackageManager(this IServiceCollection services, string vosPath, PackageManagerOptions options = null)
            => services.VosPackageManager(vosPath.ToVosReference(), options);
    }
}

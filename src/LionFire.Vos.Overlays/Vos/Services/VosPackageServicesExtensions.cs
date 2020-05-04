using LionFire.MultiTyping;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Packages;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Services
{
    public static class VosPackageServicesExtensions
    {
        #region Package Providers

        public static IServiceCollection VosPackageProvider(this IServiceCollection services, VosReference vosReference, PackageProviderOptions options = null)
        {
            services.InitializeVob<IServiceProvider>(vosReference, (v, serviceProvider) =>
             {
                 v.AddPackageProvider(options);
             });
            return services;
        }
        public static IServiceCollection VosPackageProvider(this IServiceCollection services, string vosPath, PackageProviderOptions options = null)
            => services.VosPackageProvider(vosPath.ToVosReference(), options);

        #endregion
     
    }
}

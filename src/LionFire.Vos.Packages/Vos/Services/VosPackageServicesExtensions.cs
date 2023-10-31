using LionFire.DependencyMachines;
using LionFire.MultiTyping;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Packages;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Hosting;

public static class VosPackageServicesExtensions
{

    #region Package Providers

    public static IServiceCollection VosPackageProvider(this IServiceCollection services, VobReference vobReference, PackageProviderOptions options = null)
    {
        if (options == null)
        {
            options = new()
            {
                Name = vobReference.MakeAbsolute().ToString(),
            };
        }

        services.InitializeVob<IServiceProvider>(vobReference, (v, serviceProvider) =>
         {
             v.AddPackageProvider(options);
         }, key: $"{vobReference} PackageProvider", configure: c => c
            .Provide($"{vobReference} PackageProvider")
            .After("vos:")
            );
        return services;
    }
    //public static IServiceCollection VosPackageProvider(this IServiceCollection services, string vosPath, PackageProviderOptions options = null)
        //=> services.VosPackageProvider(vosPath.ToVobReference(), options);

    #endregion
 
}

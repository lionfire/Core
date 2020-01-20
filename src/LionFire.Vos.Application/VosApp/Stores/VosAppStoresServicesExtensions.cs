using LionFire.Referencing;
using LionFire.Vos.Mounts;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Vos.Services;
using LionFire.Persistence.Filesystem;
using LionFire.Vos.Stores;
using LionFire.Vos;
using LionFire.Vos.Packages;

namespace LionFire.Services
{
    public static class VosAppStoresServicesExtensions
    {

        /// <summary>
        ///  Uses default options: VosAppDefaults.Default.VosStoresOptions
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddVosStores(this IServiceCollection services) 
            => services.AddVosStores(VosAppDefaults.Default.VosStoresOptions);

        public static IServiceCollection AddDefaultStores(this IServiceCollection services, VosAppOptions options = null)
        {
            if (options == null) options = new VosAppOptions();

            if(options.DefaultMountAppBase) services.VosMount(LionPath.Combine(VosPaths.Stores, VosStoreNames.AppBase), VosDiskPaths.AppBase.ToFileReference(), new MountOptions
            {
                IsExclusive = true,
                ReadPriority = 1,
            });
            return services;
        }
    }
}

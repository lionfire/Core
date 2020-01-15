using LionFire.Vos;
using LionFire.Vos.Internals;
using LionFire.Vos.Packages;
using LionFire.Vos.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class StoresServicesExtensions
    {
        public static IServiceCollection AddVosStores(this IServiceCollection services, VosStoresOptions options, Action<IVosStoresBuilder> configurator = null)
        {
            var vsc = new VosStoresBuilder(services, options);

            services.InitializeVob(options.StoresLocation, (s, v) =>
            {
                v.AddPackageManager(options.PackageManagerOptions);
            });

            services.InitializeRootVob(root =>
            {
                root.AddOwn(_ => options); // REVIEW - Register with ServiceDirectory instead?
            });

            configurator?.Invoke(vsc);

            return services;
        }

        public static IServiceCollection MountAllStores(this IServiceCollection services)
        {
            services.InitializeRootVob((serviceProvider, vob) =>
            {
                //var vosOptions = serviceProvider.GetService<VosOptions>();
                //if (vosOptions == null) return;

                //var rootOptions = vosOptions[vob.Root.RootName]

                //var mounter = vob.GetRequiredService<StoreMounter>();
                ////vob.GetOwn < VobMounts >()
                //foreach (var store in vob["$Internals/stores"])
                //{
                //    mounter.Mount(store);
                //}
            });
            throw new NotImplementedException();
            return services;
        }
    }
}

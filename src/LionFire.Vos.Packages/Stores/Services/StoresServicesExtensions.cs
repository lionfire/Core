﻿#if UNUSED
using LionFire.Vos;
using LionFire.Vos.Internals;
using LionFire.Vos.Packages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace LionFire.Services
{
    public static class StoresServicesExtensions
    {
        //public static IServiceCollection AddVosStores(this IServiceCollection services, VosStoresOptions options, Action<IVosStoresBuilder> config = null)
        //{
        //    var builder = new VosStoresBuilder(services, options);

        //    services.InitializeVob(options.StoresLocation, (s, v) =>
        //    {
        //        v.AddPackageManager(options.PackageManagerOptions);
        //    });

        //    services.InitializeRootVob(root =>
        //    {
        //        root.AddOwn(_ => options); // REVIEW - Register with ServiceDirectory instead?
        //    });

        //    config?.Invoke(builder);

        //    return services;
        //}

        //public static IServiceCollection MountAllStores(this IServiceCollection services)
        //{
        //    services.InitializeRootVob((serviceProvider, vob) =>
        //    {
        //        //var vosOptions = serviceProvider.GetService<VosOptions>();
        //        //if (vosOptions == null) return;

        //        //var rootOptions = vosOptions[vob.Root.RootName]

        //        //var mounter = vob.GetRequiredService<StoreMounter>();
        //        ////vob.AcquireOwn < VobMounts >()
        //        //foreach (var store in vob["$Internals/stores"])
        //        //{
        //        //    mounter.Mount(store);
        //        //}
        //    });
        //    throw new NotImplementedException();
        //    //return services;
        //}

        
    }
}
#endif
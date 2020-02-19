using LionFire.Assets;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Assets;
using LionFire.Vos.Assets.Handles;
using LionFire.Vos.Assets.Persisters;
using LionFire.Vos.Collections.ByType;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Vos.Collections;

namespace LionFire.Services
{


    public static class VosAssetServicesExtensions
    {
        public static IServiceCollection AddAssets(this IServiceCollection services, VosAssetOptions options = null, IVosReference contextVob = null)
        {
            var assetsRoot = contextVob?.Path ?? "assets";
            services
                .AddSingleton<IReadHandleProvider<IAssetReference>, VosAssetHandleProvider>()
                .AddSingleton<IReadWriteHandleProvider<IAssetReference>, VosAssetHandleProvider>()
                .VobEnvironment("assets", assetsRoot)
                .InitializeVob("$assets".ToVosReference(), (serviceProvider, vob) =>
                {
                    vob.AddOwn(v => new CollectionsByTypeManager(v));
                })

                .Configure<VosAssetOptions>(o => { })
                .AddSingleton(s => s.GetService<IOptionsMonitor<VosAssetOptions>>()?.CurrentValue)

                .TryAddEnumerableSingleton<ICollectionTypeProvider, CollectionsByTypeManager>()

                .AddSingleton<VosAssetHandleProvider>()
                .AddSingleton<IReadHandleProvider<IAssetReference>, VosAssetHandleProvider>(s => s.GetRequiredService<VosAssetHandleProvider>())
                .AddSingleton<IReadWriteHandleProvider<IAssetReference>, VosAssetHandleProvider>(s => s.GetRequiredService<VosAssetHandleProvider>())

                .AddSingleton<VosAssetPersisterProvider>()
                .AddSingleton<IPersisterProvider<IAssetReference>, VosAssetPersisterProvider>(s => s.GetRequiredService<VosAssetPersisterProvider>())
            ;
            return services;
        }

        public static IServiceCollection AddAssetPersister(this IServiceCollection services, VosAssetOptions options = null, IVosReference contextVob = null)
        {
            services.InitializeVob(contextVob ?? "".ToVosReference(), (serviceProvider, vob) =>
            {
                vob.AddOwn<VosAssetPersister>(v =>
                {
                    return (VosAssetPersister)ActivatorUtilities.CreateInstance(serviceProvider, typeof(VosAssetPersister), options ?? new VosAssetOptions());
                });
            });
            return services;
        }
    }
}

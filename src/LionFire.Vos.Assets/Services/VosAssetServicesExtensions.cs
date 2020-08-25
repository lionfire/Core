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
using LionFire.Types;
using LionFire.DependencyMachines;

namespace LionFire.Services
{
    public static class VosAssetServicesExtensions
    {
        

            public static IServiceCollection AddAssets(this IServiceCollection services, VosAssetOptions options = null, VobReference contextVob = null)
        {
            var assetsRoot = contextVob?.Path ?? "assets";
            services
                .AddTypeNameRegistry()

                .AddSingleton<VosAssetHandleProvider>() 
                .AddSingleton<IReadHandleProvider<IAssetReference>>(sp=>sp.GetRequiredService<VosAssetHandleProvider>())
                .AddSingleton<IReadWriteHandleProvider<IAssetReference>>(sp => sp.GetRequiredService<VosAssetHandleProvider>())
                .VobEnvironment("assets", assetsRoot)
                .InitializeVob<IServiceProvider>("$assets", (vob, serviceProvider) =>
                {
                    vob.AddOwn<ICollectionTypeProvider>(v => new CollectionsByTypeManager(v, serviceProvider.GetRequiredService<TypeNameRegistry>()));
                }, key: "vos:$assets<ICollectionTypeProvider>", configure: c => c.DependsOn("vos:/<VobEnvironment>/*"))

                .Configure<VosAssetOptions>(o => { })
                .AddSingleton(s => s.GetService<IOptionsMonitor<VosAssetOptions>>()?.CurrentValue)

                .TryAddEnumerableSingleton<ICollectionTypeProvider, CollectionsByTypeManager>()

                
                .AddSingleton<VosAssetHandleProvider>()
                .AddSingleton<IReadHandleProvider<IAssetReference>, VosAssetHandleProvider>(s => s.GetRequiredService<VosAssetHandleProvider>())
                .AddSingleton<IReadWriteHandleProvider<IAssetReference>, VosAssetHandleProvider>(s => s.GetRequiredService<VosAssetHandleProvider>())

                .AddSingleton<VosAssetPersisterProvider>()
                .AddSingleton<IPersisterProvider<IAssetReference>, VosAssetPersisterProvider>(s => s.GetRequiredService<VosAssetPersisterProvider>())

                .AddAssetPersister(options, contextVob)
            ;
            return services;
        }

        public static IServiceCollection AddAssetPersister(this IServiceCollection services, VosAssetOptions options = null, VobReference contextVob = null)
        {
            //.InitializeVob("/", v => v.AddOwn<VosAssetPersister>(), p => p.Key = $"/<VosAssetPersister>")

            var vob = contextVob ?? "/".ToVobReference();
            services.InitializeVob<IServiceProvider>(vob, (vob, serviceProvider) =>
            {
                vob.AddOwn<VosAssetPersister>(v =>
                {
                    return (VosAssetPersister)ActivatorUtilities.CreateInstance(serviceProvider, typeof(VosAssetPersister), options ?? new VosAssetOptions());
                });
                return;
            }, c=>c.Key = $"{vob}<{typeof(VosAssetPersister).Name}> ");
            return services;
        }
    }
}

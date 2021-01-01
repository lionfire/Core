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
using LionFire.Services;

namespace LionFire.Hosting
{

    public static class VosAssetServicesExtensions
    {
        public static IServiceCollection AddAssets(this IServiceCollection services, VosAssetOptions options = null)
        {
            options ??= VosAssetOptions.Default;
            var assetsRoot = options.AssetsRootEnvironmentVariable 
                ?? throw new ArgumentException($"{nameof(options)}.{nameof(options.AssetsRootEnvironmentVariable)}");
            

            services
                .AddTypeNameRegistry()

                .VobEnvironment(options.AssetsRootEnvironmentVariable, options.AssetsRootEnvironmentVariableValue)

                .AddSingleton<VosAssetHandleProvider>()
                .AddSingleton<IReadHandleProvider<IAssetReference>>(sp => sp.GetRequiredService<VosAssetHandleProvider>())
                .AddSingleton<IReadWriteHandleProvider<IAssetReference>>(sp => sp.GetRequiredService<VosAssetHandleProvider>())
                .InitializeVob<IServiceProvider>(options.AssetsRootEnvironmentVariableReference, (vob, serviceProvider) =>
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

                .AddVosAssetPersister(options)

                .AddSingleton<IReferenceTranslator<IAssetReference, IVobReference>, VosAssetsReferenceTranslator>()
            ;
            return services;
        }

        public static IServiceCollection AddVosAssetPersister(this IServiceCollection services, VosAssetOptions options = null)
        {
            options ??= VosAssetOptions.Default;

            var vob = options.PersisterLocation.ToVobReference();
            services.InitializeVob<IServiceProvider>(vob, (vob, serviceProvider) =>
            {
                vob.AddOwn<VosAssetPersister>(v =>
                {
                    return (VosAssetPersister)ActivatorUtilities.CreateInstance(serviceProvider, typeof(VosAssetPersister), options ?? new VosAssetOptions());
                });
                return;
            }, c => c.Key = $"{vob}<{typeof(VosAssetPersister).Name}> ");
            return services;
        }
    }
}

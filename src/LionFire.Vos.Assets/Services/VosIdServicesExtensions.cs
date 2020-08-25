using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Vos;
using LionFire.Vos.Collections.ByType;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using LionFire.Vos.Collections;
using LionFire.Types;
using LionFire.DependencyMachines;
using LionFire.Vos.Id.Persisters;
using LionFire.Data.Id;
using LionFire.Vos.Id.Handles;

namespace LionFire.Services
{
    public static class VosIdServicesExtensions
    {
        public static IServiceCollection AddIdPersistence(this IServiceCollection services, VosIdPersisterOptions options = null, VobReference contextVob = null)
        {
            var idRoot = contextVob?.Path ?? "id";
            services
                .AddTypeNameRegistry()

                .AddSingleton<VosIdHandleProvider>()
                .AddSingleton<IReadHandleProvider<IIdReference>>(sp => sp.GetRequiredService<VosIdHandleProvider>())
                .AddSingleton<IReadWriteHandleProvider<IIdReference>>(sp => sp.GetRequiredService<VosIdHandleProvider>())
                .VobEnvironment("id", idRoot)
                .InitializeVob<IServiceProvider>("id", (vob, serviceProvider) =>
                {
                    vob.AddOwn<ICollectionTypeProvider>(v => new CollectionsByTypeManager(v, serviceProvider.GetRequiredService<TypeNameRegistry>()));
                }, key: "vos:id<ICollectionTypeProvider>", configure: c => c.DependsOn("vos:/<VobEnvironment>/*"))

                .Configure<VosIdPersisterOptions>(o => { })
                .AddSingleton(s => s.GetService<IOptionsMonitor<VosIdPersisterOptions>>()?.CurrentValue)

                .TryAddEnumerableSingleton<ICollectionTypeProvider, CollectionsByTypeManager>()


                .AddSingleton<VosIdHandleProvider>()
                .AddSingleton<IReadHandleProvider<IIdReference>, VosIdHandleProvider>(s => s.GetRequiredService<VosIdHandleProvider>())
                .AddSingleton<IReadWriteHandleProvider<IIdReference>, VosIdHandleProvider>(s => s.GetRequiredService<VosIdHandleProvider>())

                .AddSingleton<VosIdPersisterProvider>()
                .AddSingleton<IPersisterProvider<IIdReference>, VosIdPersisterProvider>(s => s.GetRequiredService<VosIdPersisterProvider>())

                .AddIdPersister(options, contextVob)
            ;
            return services;
        }

        public static IServiceCollection AddIdPersister(this IServiceCollection services, VosIdPersisterOptions options = null, VobReference contextVob = null)
        {
            var vob = contextVob ?? "/".ToVobReference();
            services.InitializeVob<IServiceProvider>(vob, (vob, serviceProvider) =>
            {
                vob.AddOwn<VosIdPersister>(v =>
                {
                    return (VosIdPersister)ActivatorUtilities.CreateInstance(serviceProvider, typeof(VosIdPersister), options ?? new VosIdPersisterOptions());
                });
                return;
            }, c => c.Key = $"{vob}<{typeof(VosIdPersister).Name}> ");
            return services;
        }

    }
}

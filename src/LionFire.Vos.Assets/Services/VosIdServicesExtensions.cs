﻿using LionFire.Persistence.Handles;
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
using LionFire.Hosting;

namespace LionFire.Hosting;

public static class VosIdServicesExtensions
{
    public const string DefaultIdPath = "/id";

    public static IServiceCollection AddIdPersistence(this IServiceCollection services, VosIdPersisterOptions options = null, VobReference contextVob = null)
    {
        var idRoot = contextVob?.Path ?? DefaultIdPath;
        contextVob ??= idRoot.ToVobReference();

        services
            .VobEnvironment("id", idRoot)

            .AddTypeNameRegistry()

            .InitializeVob<IServiceProvider>(idRoot, (vob, serviceProvider) =>
            {
                vob.AddOwn<ICollectionTypeProvider>(v => new CollectionsByTypeManager(v, serviceProvider.GetRequiredService<TypeNameRegistry>()));
            }, key: contextVob + "<ICollectionTypeProvider>", configure: c => c.DependsOn("vos:/<VobEnvironment>/*"))

            .Configure<VosIdPersisterOptions>(o => { })
            .AddSingleton(s => s.GetService<IOptionsMonitor<VosIdPersisterOptions>>()?.CurrentValue)

            .TryAddEnumerableSingleton<ICollectionTypeProvider, CollectionsByTypeManager>()

            .AddSingleton<VosIdHandleProvider>()
            .AddSingleton<IReadHandleProvider<IIdReference>>(sp => sp.GetRequiredService<VosIdHandleProvider>())
            .AddSingleton<IReadHandleCreator<IIdReference>>(sp => sp.GetRequiredService<VosIdHandleProvider>())
            .AddSingleton<IReadWriteHandleProvider<IIdReference>>(sp => sp.GetRequiredService<VosIdHandleProvider>())

            .AddSingleton<VosIdPersisterProvider>()
            .AddSingleton<IPersisterProvider<IIdReference>, VosIdPersisterProvider>(s => s.GetRequiredService<VosIdPersisterProvider>())

            .AddIdPersister(options, contextVob)
        ;
        return services;
    }

    private static IServiceCollection AddIdPersister(this IServiceCollection services, VosIdPersisterOptions options = null, VobReference contextVob = null)
    {
        // TODO: Set VosIdPersister on vos:$id instead of vos:/.
        var vob = "/".ToVobReference(); // TODO: contextVob ?? DefaultIdPath.ToVobReference();

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

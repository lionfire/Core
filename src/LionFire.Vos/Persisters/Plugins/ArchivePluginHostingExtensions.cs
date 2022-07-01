#nullable enable
using LionFire.DependencyMachines;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Services;
using LionFire.Vos;
using LionFire.Vos.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Hosting;

public static class ArchivePluginHostingExtensions
{
    public static IServiceCollection ArchivePlugin(this IServiceCollection services, VobReference vobReference, ArchivePluginOptions? options = null)
    {
        services.InitializeVob<IServiceProvider>(vobReference, (v, serviceProvider) =>
        {
            v.AddArchivePlugin(serviceProvider, options);
        }, key: $"{vobReference} ArchivePlugin", configure: c => c.Provide($"{vobReference} ArchivePlugin"));
        return services;
    }

    public static IVob AddArchivePlugin(this IVob vob, IServiceProvider serviceProvider, ArchivePluginOptions ? options = null)
    {
        options ??= new ArchivePluginOptions();

        var archivePlugin = ActivatorUtilities.CreateInstance<ArchivePlugin>(serviceProvider, vob, options);

        vob.AddOwn<ArchivePlugin>(archivePlugin);

        //#region Deprecated

        //// REVIEW - is this still needed?  VobNode seems cleaner.  Calling method can add to ServiceDirectory if desired.

        //// Get the ServiceDirectory (crawls up the Vob tree).  ServiceDirectories can currently only be created at a RootVob.
        //// Registers the PackageProvider at the given path
        //// Name must be unique among PackageProviders.  If omitted, it is treated as empty.
        //vob.GetRequiredService<ServiceDirectory>().Register(archivePlugin, name: options?.Name);

        //#endregion

        return vob;
    }
}

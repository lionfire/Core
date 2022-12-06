#nullable enable
using LionFire;
using LionFire.DependencyMachines;
using LionFire.ExtensionMethods.Acquisition;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.Expanders;
using LionFire.Services;
using LionFire.Vos;
using LionFire.Vos.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Hosting;

public static class ExpansionMounterHostingExtensions
{
    public static IServiceCollection ArchiveAdapter(this IServiceCollection services, VobReference vobReference, ExpansionMountOptions? options = null)
    {
        services.InitializeVob<IServiceProvider>(vobReference, (v, serviceProvider) =>
        {
            v.AddArchiveAdapter(serviceProvider, options);
        }, key: $"{vobReference} ArchivePlugin", configure: c => c.Provide($"{vobReference} ArchivePlugin"));
        return services;
    }

    public static IVob AddArchiveAdapter(this IVob vob, IServiceProvider serviceProvider, ExpansionMountOptions? options = null)
    {
        options ??= new ExpansionMountOptions();

        var archivePlugin = ActivatorUtilities.CreateInstance<ExpansionMounter>(serviceProvider, vob, options);

        vob.AddOwn<ExpansionMounter>(archivePlugin);

        var h = vob.Acquire<IVob, Handlers<BeforeListEventArgs>>();
        if (h == null)
        {
            h = new Handlers<BeforeListEventArgs>();
            vob.SetAcquirable<IVob, Handlers<BeforeListEventArgs>>(h);
        }
        h.AddHandler(archivePlugin.BeforeListHandler);

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

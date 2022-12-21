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

public static class ExpanderMounterHostingX
{
    public static IServiceCollection AddExpanderMounter(this IServiceCollection services, VobReference vobReference, ExpansionMountOptions? options = null)
    {
        services.InitializeVob<IServiceProvider>(vobReference, (v, serviceProvider) =>
        {
            v.AddExpanderMounter(serviceProvider, options);
        }, key: $"{vobReference} ArchivePlugin", configure: c => c.Provide($"{vobReference} ArchivePlugin"));
        return services;
    }

    public static IVob AddExpanderMounter(this IVob vob, IServiceProvider serviceProvider, ExpansionMountOptions? options = null)
    {
        options ??= new ExpansionMountOptions();

        var expanderMounter = ActivatorUtilities.CreateInstance<ExpanderMounter>(serviceProvider, vob, options);

        vob.AddOwn(expanderMounter);

        AddHandler<BeforeListEventArgs>(vob, expanderMounter, expanderMounter.BeforeListHandler);
        AddHandler<BeforeRetrieveEventArgs>(vob, expanderMounter, expanderMounter.BeforeRetrieveHandler);

        //#region Deprecated

        //// REVIEW - is this still needed?  VobNode seems cleaner.  Calling method can add to ServiceDirectory if desired.

        //// Get the ServiceDirectory (crawls up the Vob tree).  ServiceDirectories can currently only be created at a RootVob.
        //// Registers the PackageProvider at the given path
        //// Name must be unique among PackageProviders.  If omitted, it is treated as empty.
        //vob.GetRequiredService<ServiceDirectory>().Register(archivePlugin, name: options?.Name);

        //#endregion

        return vob;

        static void AddHandler<TArgs>(IVob vob, ExpanderMounter archivePlugin, Func<TArgs, Task> handler)
        {
            var h = vob.Acquire<IVob, Handlers<TArgs>>();
            if (h == null)
            {
                h = new Handlers<TArgs>();
                vob.SetAcquirable<IVob, Handlers<TArgs>>(h);
            }
            h.AddHandler(handler);
        }
    }
}

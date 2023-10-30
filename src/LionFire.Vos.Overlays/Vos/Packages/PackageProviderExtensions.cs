using LionFire.MultiTyping;
using Microsoft.Extensions.DependencyInjection;
using System;
using LionFire.Vos.Services;
using LionFire.Vos.Mounts;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Referencing;
using LionFire.Persistence;
using LionFire.FlexObjects;

namespace LionFire.Vos.Packages;

public static class PackageProviderExtensions
{
    public static IVob AddPackageProvider(this IVob vob, PackageProviderOptions options = null)
    {
        options ??= new PackageProviderOptions();

        var packageProvider = new PackageProvider(vob, options: options);

        vob.AddOwn<PackageProvider>(packageProvider);

        #region Deprecated

        // REVIEW - is this still needed?  VobNode seems cleaner.  Calling method can add to ServiceDirectory if desired.

        // Get the ServiceDirectory (crawls up the Vob tree).  ServiceDirectories can currently only be created at a RootVob.
        // Registers the PackageProvider at the given path
        // Name must be unique among PackageProviders.  If omitted, it is treated as empty.

#error NEXT: Named ServiceDirectory dictionary on Flex
        // NEXT: Sunset ServiceDirectory
        vob.GetRequiredService<ServiceDirectory>().Register(packageProvider, name: options?.Name);
#error NEXT
        // NEXT: do I want IVob to be IFlex? Alternative: IHasFlex
        vob.Add(options?.Name, packageProvider);

        #endregion

        return vob;
    }

    public static PackageProvider AsPackageProvider(this IVob vob) => throw new NotImplementedException();// vob.Get<PackageProvider>();

    /// <summary>
    /// This is run at init time (and can be re-run if the EffectiveAutoAddPackagesFromStoresSubpath has packages added to it) to populate a PackageActivator's
    /// available folder with packages available in each store's EffectiveAutoAddPackagesFromStoresSubpath.
    /// </summary>
    /// <param name="packageProvider"></param>
    /// <param name="packagesVob">The children of this will be scanned and used as available packages</param>
    /// <param name="mountOptions"></param>
    /// <param name="filter">Filter here based on things like whether there is an appropriate package metadata file under each child.</param>
    /// <returns></returns>
    public static async Task TryAutoRegisterPackages(this PackageProvider packageProvider, IVob packagesVob = null, VobMountOptions mountOptions = null,
        Func<IVobReference, Task<bool>> filter = null
        //, bool? requiredExistResult = true
        )
    {
        var root = packageProvider.Vob.Root;
        PackageProviderOptions options = packageProvider.Options;
        packagesVob ??= root["$stores"];


        if (!packageProvider.IsAutoRegisterAvailablePackagesEnabled) return;
        var autoAddSubPath = options.EffectiveAutoAddPackagesFromStoresSubpath;

        //if (requiredExistResult == false) throw new ArgumentException($"{nameof(requiredExistResult)} cannot be false.");

        mountOptions ??= options?.DefaultMountOptions;

        var hList = packagesVob.Reference.GetListingsHandle();
        var result = await hList.Get().ConfigureAwait(false);
        var listings = result.Value.Value;

        //#error NEXT: Look at what's going on at $stores.  There should be multiple stores mounted as Vobs under $stores.  Each one should have an exclusive ReadMount.  The ReadMount name should match the store name, and the WriteMount, if any should also match.
        foreach (var packagesVobChild in listings)
        {
            //IMount mount = storeVob.AcquireOwn<VobMounts>().ReadMounts.Single().Value;
            //.Select(kvp => kvp.Value))

            var storeName = packagesVobChild.Name;
            //var storeName = mount.MountPoint.Name;

            if (options.StoreNameBlacklist != null && options.StoreNameBlacklist.Contains(storeName)) continue; // TOTEST
            if (options.StoreNameWhitelist != null && !options.StoreNameWhitelist.Contains(storeName)) continue; // TOTEST

            var potentialPackageSourceTarget = packagesVob.Reference.GetChild(packagesVobChild.Name); // Example: "file:c:\Program Files (x86)\Org\App\Base"
            //Child.GetChild(autoAddSubPath);  

            // REVIEW - is this a working / recommended way to determine if the folder exists?

            if (filter != null && !(await filter(potentialPackageSourceTarget).ConfigureAwait(false))) continue;
            //if (await potentialPackageSourceTarget.GetReadHandle<IDirectory>().Exists().ConfigureAwait(false) != requiredExistResult) continue;

            packageProvider.AvailableRoot[storeName].Mount(root[$"$stores/{storeName}/{autoAddSubPath}"], mountOptions);
        }
    }

}

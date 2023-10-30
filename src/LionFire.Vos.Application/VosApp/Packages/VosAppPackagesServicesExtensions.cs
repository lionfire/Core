using LionFire.Hosting;
using LionFire.Services;
using LionFire.Vos.Mounts;
using LionFire.Vos.Packages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.VosApp;



public static class VosAppPackagesServicesExtensions
{
    public static IServiceCollection VosPackageProvider(this IServiceCollection services, string name, bool autoSource = true, bool trustedOnly = false, bool exclusive = false, bool autoActivate = false)
    {
        var options = new PackageProviderOptions
        {
            Mode = PackageProviderMode.Unspecified | (exclusive ? PackageProviderMode.Single : PackageProviderMode.Overlay | PackageProviderMode.Active),
            AutoActivate = autoActivate,
        };

        VobReference packagePath = VosPackageLocations.GetPackageProviderPath(name);
        services.VosPackageProvider(packagePath, options);
        return services;
    }

    #region Package Sources


    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="services"></param>
    ///// <param name="packageProviderName"></param>
    ///// <param name="packageProviderOptions"></param>
    ///// <param name="mountOptions"></param>
    ///// <param name="requiredExistResult">true if required to be confirmed to exist, null if can't confirm whether exists or not.  False should not be used</param>
    ///// <returns></returns>
    //public static IServiceCollection AddExistingPackageSources(this IServiceCollection services, string packageProviderName, PackageProviderOptions packageProviderOptions = null, MountOptions mountOptions = null, bool? requiredExistResult = true)
    //{
    //    services.InitializeRootVob((Action<IRootVob>)(async root =>
    //    {

    //        await root.AddExistingPackageSources(packageProviderName, packageProviderOptions, mountOptions, requiredExistResult);
    //    }), prerequisites: VosAppInitialization.Stores.AsEnumerable());

    //    return services;
    //}

    #endregion

    #region Package Providers

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="services"></param>
    ///// <param name="name">Name of the package provider</param>
    ///// <param name="autoSource"></param>
    ///// <param name="trustedOnly">If true, only mount from trusted (i.e. first party) sources.  If false, allow user data to augment available data.</param>
    ///// <param name="exclusive">If true, Single mode, otherwise Overlay | Active</param>
    ///// <param name="autoActivate">If true, automatically activate all available packages</param>
    ///// <returns></returns>
    //public static IServiceCollection AddPackageProvider(this IServiceCollection services, string name, bool autoSource = true, bool trustedOnly = false, bool exclusive = false, bool autoActivate = false)
    //{
    //    var options = new PackageProviderOptions
    //    {
    //        Mode = PackageProviderMode.Unspecified | (exclusive ? PackageProviderMode.Single : PackageProviderMode.Overlay | PackageProviderMode.Active),
    //        AutoActivate = autoActivate,
    //    };

    //    services.AddPackageProvider(name, options);
    //    return services;
    //}

    //public static IServiceCollection AddPackageProvider(this IServiceCollection services, string name, PackageProviderOptions options) 
    //    => services.InitializeVob(VosPackageLocations.GetPackageProviderPath(name), 
    //        vob=> vob.AddPackageProvider(options ?? new PackageProviderOptions()));

    //public static IServiceCollection AddPackageProviders(this IServiceCollection services, params string[] names)
    //{
    //    foreach (var name in names)
    //    {
    //        services.AddPackageProvider(name);
    //    }
    //    return services;
    //}

    #endregion


    //public static IServiceCollection AddExeDirBasePackage(this IServiceCollection services, string storeName = StoreNames.ExeDir)
    //{
    //    return services.TryAddAvailablePackage("$DataPackageManager", $"$stores/{storeName}", new MountOptions(100, null));
    //    //return services.InitializeRootVob((serviceProvider, root) =>
    //    //{
    //    //    var packageManagerVob = "$DataPackageManager".QueryVob();
    //    //    if (packageManagerVob == null) return;
    //    //    var packageManager = packageManagerVob.AsPackageManager();
    //    //    if (packageManager == null) return;

    //    //    var target = $"$stores/{name}".QueryVob();
    //    //    if (target == null) return;

    //    //    packageManager?.AvailableRoot.Mount(target, new MountOptions(100, null));
    //    //});
    //}

    //public static IServiceCollection TryAddAvailablePackage(this IServiceCollection services, VobReference packageManager, VobReference target, MountOptions options = null)
    //{
    //    return services.InitializeRootVob((serviceProvider, root) =>
    //    {
    //        var packageManagerVob = packageManager.QueryVob();
    //        if (packageManagerVob == null) return; // TOLOG
    //        var packageManagerObj = packageManagerVob.Get<PackageProvider>();
    //        if (packageManagerObj == null) return; // TOLOG

    //        var targetVob = target.QueryVob();
    //        if (targetVob == null) return; // TOLOG

    //        packageManagerObj?.AvailableRoot.Mount(targetVob, options);
    //    });
    //}
}


#nullable enable
using LionFire.DependencyMachines;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Packages;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Services
{
    //public class NamedTypeDependency<T>
    //{
    //    public string Name { get; set; }
    //    public Type Type => typeof(T);

    //}

    //public class Dependencies
    //{
    //    List<IReadWriteHandle> ReadWriteHandles { get; set; }
    //}

    public static class VosAppPackageProviderServicesExtensions
    {
        public static IServiceCollection AddPackageSourceFromStore(this IServiceCollection services, string storeName, string packageProviderName, VobMountOptions? mountOptions = null, string? rootName = null)
        {
            throw new NotImplementedException();
            //return services.InitializeVob(VosPaths.GetRootPath(rootName), root =>
            //     (root[VosPackageLocations.GetPackageProviderPath(packageProviderName)].AsPackageProvider()?.AvailableRoot
            //        ?? throw new NotFoundException($"Could not find package provider '{packageProviderName}''s mount point for available packages.  Is this packageProvider registered?"))
            //        .Mount(root[$"$stores/{storeName}"], mountOptions),
            //     c => c.DependsOn(VosAppInitStage.PackageProviders)
            //        .Contributes(VosAppInitStage.PackageSources)
            //     );
        }

        #region All Defaults

        public static IServiceCollection DefaultStoresAvailableToCore(this IServiceCollection services)
        {
            return services
                .AddExeDirToBasePackages()
                ;
        }
        public static IServiceCollection DefaultStoresAvailableToData(this IServiceCollection services)
        {
            //var packageManagerName = VosAppPackageManagers.Data;
            return services
                ;
        }

        #region Individual Defaults

        public static IServiceCollection AddExeDirToBasePackages(this IServiceCollection services, VobMountOptions? mountOptions = null)
            => services
            .AddPackageSourceFromStore(StoreNames.ExeDir, VosAppPackageProviderNames.Base, mountOptions ?? new VobMountOptions(100, null))
            ;

        #endregion

        #endregion

    }
}

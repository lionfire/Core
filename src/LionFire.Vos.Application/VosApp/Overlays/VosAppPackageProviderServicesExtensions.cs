using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Packages;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Services
{
    public static class VosAppPackageProviderServicesExtensions
    {
        public static IServiceCollection AddStoreAsPackageSource(this IServiceCollection services, string storeName, string packageProviderName, MountOptions mountOptions = null)
        {
            return services.InitializeRootVob(root =>
            {
                var availableVob = root[VosPackageLocations.GetPackageProviderPath(packageProviderName)].AsPackageProvider()?.AvailableRoot;
                if (availableVob == null) throw new NotFoundException("Mount point not found"); // TODO: Allow silent fail?
                availableVob.Mount(root[$"$stores/{storeName}"], mountOptions);
            }, provides: VosAppInitialization.OverlaySources.AsEnumerable());
        }

        #region All Defaults

        public static IServiceCollection DefaultStoresAvailableToBase(this IServiceCollection services)
        {
            //var packageManagerName = "base";
            return services
                .ExeDirAvailableToBase()
                ;
        }
        public static IServiceCollection DefaultStoresAvailableToData(this IServiceCollection services)
        {
            //var packageManagerName = VosAppPackageManagers.Data;
            return services
                ;
        }

        #region Individual Defaults

        public static IServiceCollection ExeDirAvailableToBase(this IServiceCollection services, MountOptions mountOptions = null)
            => services.AddStoreAsPackageSource(StoreNames.ExeDir, VosAppPackageProviderNames.Core, mountOptions ?? new MountOptions(100, null));

        #endregion

        #endregion

    }
}

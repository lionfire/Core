using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Overlays;
using LionFire.Vos.VosApp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Services
{
    public static class VosAppOverlayServicesExtensions
    {
        public static IServiceCollection AddOverlayStack(this IServiceCollection services, string name, OverlayStackOptions options = null)
        {
            options ??= new OverlayStackOptions();

            services.InitializeRootVob((serviceProvider, root) =>
            {
                root[VosOverlayDirs.GetOverlayStackPath(name)].AddOverlayStack(options);
            });

            if (options.AddExistingOverlaySources) { services.AddExistingOverlaySources(name, overlayStackOptions: options); }

            return services;
        }

        public static IServiceCollection AddStoreAsOverlaySource(this IServiceCollection services, string storeName, string overlayStackName, MountOptions mountOptions = null)
        {
            return services.InitializeRootVob(root =>
            {
                var availableVob = root[VosOverlayDirs.GetOverlayStackPath(overlayStackName)].AsOverlayStack()?.AvailableRoot;
                if (availableVob == null) throw new NotFoundException("Mount point not found"); // TODO: Allow silent fail?
                availableVob.Mount(root[$"$stores/{storeName}"], mountOptions);
            }, provides: VosAppInitialization.OverlaySources.AsEnumerable());
        }

        #region All Defaults

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="overlayStackName"></param>
        /// <param name="overlayStackOptions"></param>
        /// <param name="mountOptions"></param>
        /// <param name="requiredExistResult">true if required to be confirmed to exist, null if can't confirm whether exists or not.  False should not be used</param>
        /// <returns></returns>
        public static IServiceCollection AddExistingOverlaySources(this IServiceCollection services, string overlayStackName, OverlayStackOptions overlayStackOptions = null, MountOptions mountOptions = null, bool? requiredExistResult = true)
        {
            services.InitializeRootVob(async root =>
            {
                await root.AddExistingOverlaySources(overlayStackName, overlayStackOptions, mountOptions, requiredExistResult);
            }, prerequisites: VosAppInitialization.Stores.AsEnumerable());

            return services;
        }

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
            => services.AddStoreAsOverlaySource(StoreNames.ExeDir, VosAppPackageManagers.Base, mountOptions ?? new MountOptions(100, null));

        #endregion

        #endregion

    }
}

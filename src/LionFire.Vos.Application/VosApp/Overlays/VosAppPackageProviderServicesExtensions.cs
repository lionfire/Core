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
    public class NamedTypeDependency<T>
    {
        public string Name { get; set; }
        public Type Type => typeof(T);


    }

    public class Dependencies
    {
        List<IReadWriteHandle> ReadWriteHandles { get; set; }
    }

    public static class VosAppPackageProviderServicesExtensions
    {


        public static IServiceCollection AddStoreAsPackageSource(this IServiceCollection services, string storeName, string packageProviderName, MountOptions mountOptions = null)
        {
            var x = new ObjectReadWriteHandle<RootVob>()

            //return services.Configure<DependencyMachineConfig>(o =>
            //{
            //    o.
            //});
#error NEXT: how to do this?  I need to register a Participant with a named dependency (named RootVob. key = "rootvob:<name>") and have it injected to a method (or a class).
#error Idea: make IDepMach a INamedServiceProvider?  Maybe unneeded
#error NEXT: Before DSM starts a participant, check it for IHasDependencies, and inject.  ok, INamedServiceProvider is needed if there is a named dependency.

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

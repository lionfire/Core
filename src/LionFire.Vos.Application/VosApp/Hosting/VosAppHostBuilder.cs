using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.Vos.VosApp;
using LionFire.Vos;
using LionFire.Applications;
using LionFire.Vos.Packages;
using System.Collections.Generic;
using LionFire.DependencyMachines;
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting // REVIEW - consider changing this to LionFire.Services to make it easier to remember how to create a new app
{
    public static class VosAppHostBuilder
    {
        public static IHostBuilder Create(string[] args = null, VosAppOptions options = null, bool defaultBuilder = true)
        {
            if (options == null) options = VosAppOptions.Default;

            return VosHost.Create(args, defaultBuilder: defaultBuilder)
                     .ConfigureServices((context, services) =>
                     {
                         services
                             .AddDependencyMachine()
                             .ConfigureDependencyMachine(dm =>
                             {
                                 dm.Register(DependencyStages.CreateStageChain(
                                     null,
                                     VosAppInitStage.Stores,
                                     VosAppInitStage.PackageProviders,
                                     VosAppInitStage.PackageSources));
                             })
                             .VobEnvironment("app", "/app".ToVosReference()) // TODO: VosAppLocations.App = "$app"
                             .VobEnvironment(VosPackageLocations.Packages, "/packages".ToVosReference())
                             .VobEnvironment("internal", "/_".ToVosReference())

                             .VobEnvironment("stores", "/_/stores".ToVosReference()) //.VobEnvironment("stores", "/$internal/stores".ToVosReference()) // TODO

                             //.AddVosAppStores(options?.VosStoresOptions) 

                             .VobAlias("/`", "/app") // TODO, TOTEST
                                                     //.AddVosAppDefaultMounts(options) // TODO
                                                     //.VobAlias("/`", "$AppRoot") // FUTURE?  Once environment variables are ready

                             //.AddVosAppOptions(options)
                             ;

                     })
                ;
        }

        public static AppInfo AppInfo(this IHostBuilder hostBuilder)
            => hostBuilder.Properties[typeof(AppInfo)] as AppInfo ?? throw new ArgumentException("IHostBuilder needs to have AddAppInfo() invoked on it.");

        public static IHostBuilder AddDefaultVosAppStores(this IHostBuilder hostBuilder, bool useExeDirAsAppDirIfMissing = false)
            => hostBuilder.ConfigureServices(services =>
                services
                     .AddAppDirStore(appInfo: hostBuilder.AppInfo(), useExeDirAsAppDirIfMissing: useExeDirAsAppDirIfMissing)
                     .AddExeDirStore()
                     .AddPlatformSpecificStores(hostBuilder.AppInfo()));

    }
}

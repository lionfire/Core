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
using LionFire.Settings;

namespace LionFire.Hosting // REVIEW - consider changing this to LionFire.Services to make it easier to remember how to create a new app
{
    public static class VosAppHostBuilder
    {
        #region LionFireHostBuilder

        public static LionFireHostBuilder VosApp(this LionFireHostBuilder lf)
            => lf
                    .Vos()
                    .ForHostBuilder(b => b
                        .ConfigureServices((context, services) => services.AddVosApp())
                        );


        #endregion

        //[Obsolete]
        //public static IHostBuilder Create(IConfiguration config = null, string[] args = null, VosAppOptions options = null, bool defaultBuilder = true, IHostBuilder hostBuilder = null)
        //{
        //    if (options == null) options = VosAppOptions.Default;

        //    return VosHost.Create(config, args, defaultBuilder: defaultBuilder, hostBuilder: hostBuilder)
        //             .ConfigureServices((context, services) => services.AddVosApp())
        //        ;
        //}

        public static IServiceCollection AddVosApp(this IServiceCollection services)
            => services
                 .AddDependencyMachine()
                 .ConfigureDependencyMachine(dm =>
                 {
                     dm.Register(DependencyStages.CreateStageChain(
                         null,
                         VosAppInitStage.Stores,
                         VosAppInitStage.PackageProviders,
                         VosAppInitStage.PackageSources));
                 })
                 .VobEnvironment("app", "/app".ToVobReference()) // TODO: VosAppLocations.App = "$app"
                 .VobEnvironment(VosPackageLocations.Packages, "/packages".ToVobReference())
                 .VobEnvironment("internal", "/_".ToVobReference())
                 .VobEnvironment("stores", "/_/stores".ToVobReference()) //.VobEnvironment("stores", "/$internal/stores".ToVobReference()) // TODO

                 //.AddVosAppStores(options?.VosStoresOptions) 

                 .VobAlias("/`", "/app") // TODO, TOTEST
                                         //.AddVosAppDefaultMounts(options) // TODO
                                         //.VobAlias("/`", "$AppRoot") // FUTURE?  Once environment variables are ready
                .AddSettings()

                //.AddVosAppOptions(options)
                ;

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

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

namespace LionFire.Hosting // REVIEW - consider changing this to LionFire.Services to make it easier to remember how to create a new app
{

    public static class VosAppHostBuilder
    {

        // TODO: move serializers and maybe defaultBuilder away from here

        public static IHostBuilder Create(string appName = null, string orgName = null, bool defaultBuilder = true, string[] args = null)
            => Create(args: args, options: new VosAppOptions { AppInfo = new AppInfo { AppName = appName, OrgName = orgName } }, defaultBuilder: defaultBuilder);

        public static IHostBuilder Create(string[] args = null, VosAppOptions options = null, bool defaultBuilder = true, IDependencyStateMachine dependencies = null)
        {
            if (options == null) options = VosAppOptions.Default;
                             
            return VosHost.Create(args, defaultBuilder: defaultBuilder)
                     .SetAppInfo(options?.AppInfo)
                     .ConfigureServices((context, services) =>
                     {
                         services
                             .ConfigureDependencyMachine(dm=>
                             {
                                 dm.Register(DependencyStages.CreateStageChain(
                                     VosAppInitStage.Stores, 
                                     VosAppInitStage.PackageProviders, 
                                     VosAppInitStage.PackageSources));
                             })
                             .AddSingleton<IDependencyStateMachine, IDependencyStateMachine>()
                             .VobEnvironment("app", "/app".ToVosReference()) // TODO: VosAppLocations.App = "$app"
                             .VobEnvironment(VosPackageLocations.Packages, "/packages".ToVosReference())
                             .VobEnvironment("internal", "/_".ToVosReference())

                             .VobEnvironment("stores", "/_/stores".ToVosReference()) //.VobEnvironment("stores", "/$internal/stores".ToVosReference()) // TODO

                             //.AddVosAppStores(options?.VosStoresOptions) 

                             .VobAlias("/`", "/app") // TODO, TOTEST
                                                     //.AddVosAppDefaultMounts(options) // TODO
                                                     //.VobAlias("/`", "$AppRoot") // FUTURE?  Once environment variables are ready

                             .AddVosAppOptions(options)
                             ;

                         if (options.AddDefaultStores)
                         {
                             services
                                 .AddAppDirStore(appInfo: options.AppInfo ?? context.Properties["AppInfo"] as AppInfo, useExeDirAsAppDirIfMissing: options.UseExeDirAsAppDirIfMissing)
                                 .AddExeDirStore()
                                 .AddPlatformSpecificStores(context.Configuration);
                         }
                     })
                ;
        }
    }
}

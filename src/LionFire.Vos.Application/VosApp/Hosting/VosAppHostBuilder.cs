using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.Vos.VosApp;
using LionFire.Vos;
using LionFire.Applications;
using System.Collections.Generic;
using LionFire.DependencyMachines;
using Microsoft.Extensions.Configuration;
using LionFire.Settings;
using LionFire.Vos.Packages;

namespace LionFire.Hosting; // REVIEW - consider changing this to LionFire.Services to make it easier to remember how to create a new app

public static class VosAppHostBuilder
{
    #region ILionFireHostBuilder

    public static ILionFireHostBuilder VosApp(this ILionFireHostBuilder lf)
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

    public static IServiceCollection AddVosPackages(this IServiceCollection services)
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
             .VobEnvironment(VosPackageLocations.Packages, "/packages".ToVobReference())
        ;

    public static IServiceCollection AddVosApp(this IServiceCollection services)
        => services
             .AddVosPackages()
             .VobEnvironment("app", "/app".ToVobReference()) // TODO: VosAppLocations.App = "$app"
             .VobEnvironment("internal", "/_".ToVobReference())
             .VobEnvironment("stores", "/_/stores".ToVobReference()) //.VobEnvironment("stores", "/$internal/stores".ToVobReference()) // TODO

             //.AddVosAppStores(options?.VosStoresOptions) 

             .VobAlias("/`", "/app") // TODO, TOTEST
                                     //.AddVosAppDefaultMounts(options) // TODO
                                     //.VobAlias("/`", "$AppRoot") // FUTURE?  Once environment variables are ready
            .AddSettings()

            //.AddVosAppOptions(options)
            ;

    #region // Obsolete: IHostBuilder

    public static IHostBuilder AddDefaultVosAppStores(this IHostBuilder builder, bool useExeDirAsAppDirIfMissing = false)
        => builder.ConfigureServices((context, services) =>
            services
                 .AddAppDirStore(appInfo: builder.AppInfo(), useExeDirAsAppDirIfMissing: useExeDirAsAppDirIfMissing)
                 .AddExeDirStore()
                 .AddPlatformSpecificStores(builder.AppInfo()));

    #endregion

    public static AppInfo AppInfo(this IHostApplicationBuilder builder)
        => builder.Services.BuildServiceProvider().GetService<AppInfo>() ?? throw new ArgumentException("HostApplicationBuilder needs to have AddAppInfo() already invoked on it.");

    public static IHostApplicationBuilder AddDefaultVosAppStores(this IHostApplicationBuilder builder, bool useExeDirAsAppDirIfMissing = false)
    {
        builder.Services
                 .AddAppDirStore(appInfo: builder.AppInfo(), useExeDirAsAppDirIfMissing: useExeDirAsAppDirIfMissing)
                 .AddExeDirStore()
                 .AddPlatformSpecificStores(builder.AppInfo());
        return builder;
    }
}

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Services;
using LionFire.Vos.VosApp;
using LionFire.Vos;
using LionFire.Applications;
using LionFire.Vos.Overlays;
using System.Collections.Generic;

namespace LionFire.Hosting // REVIEW - consider changing this to LionFire.Services to make it easier to remember how to create a new app
{

    public static class VosAppHost
    {

        // TODO: move serializers and maybe defaultBuilder away from here

        public static IHostBuilder Create(string appName = null, string orgName = null, bool defaultBuilder = true, string[] args = null)
            => Create(args: args, options: new VosAppOptions { AppInfo = new AppInfo { AppName = appName, OrgName = orgName } }, defaultBuilder: defaultBuilder);

        public static IHostBuilder Create(string[] args = null, VosAppOptions options = null, bool defaultBuilder = true)
        {
            if (options == null) options = VosAppOptions.Default;

            return VosHost.Create(args, defaultBuilder: defaultBuilder)
                     .SetAppInfo(options?.AppInfo)
                     .ConfigureServices((context, services) =>
                     {
                         services
                             .VobEnvironment("app", "/app".ToVosReference())
                             .VobEnvironment("overlays", "/overlays".ToVosReference())
                             .VobEnvironment("internal", "/_".ToVosReference())

                             .VobEnvironment("stores", "/_/stores".ToVosReference())
                             //.VobEnvironment("stores", "/$internal/stores".ToVosReference()) // TODO

                             .AddAppDirStore(appInfo: options.AppInfo ?? context.Properties["AppInfo"] as AppInfo, useExeDirAsAppDirIfMissing: options.UseExeDirAsAppDirIfMissing)
                             .AddExeDirStore()
                             .AddPlatformSpecificStores(context.Configuration)

                             .AddOverlayStack("core") // TEMP - let apps choose their own
                             .AddOverlayStack("data") // TEMP - let apps choose their own

                             //.AddVosAppStores(options?.VosStoresOptions) 


                             .VobAlias("/`", "/app") // TODO, TOTEST
                                                     //.AddVosAppDefaultMounts(options) // TODO
                                                     //.VobAlias("/`", "$AppRoot") // FUTURE?  Once environment variables are ready
                             ;
                     })
                ;
        }
    }
}

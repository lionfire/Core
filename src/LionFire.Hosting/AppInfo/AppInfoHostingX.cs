using LionFire.Applications;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class AppInfoHostingX
{
    public static AppInfo? GetDefaultAppInfo(this IHostEnvironment env)
    {
        Dictionary<string, string> RecognizedOrgs = new Dictionary<string, string>
        {
            ["LionFire"] = "lionfire.ca",
        };

        AppInfo? appInfo;

        if (env == null) return null;

        var index = env.ApplicationName.IndexOf('.');
        if (index <= 0) return null;

        var org = env.ApplicationName.Substring(0, index);

        if (!RecognizedOrgs.TryGetValue(org, out var orgDomain)) return null;

        appInfo = new AppInfo(env.ApplicationName.Substring(index + 1), org)
        {
            OrgDomain = orgDomain,
        };

        return appInfo;
    }

    #region Host Builder extension methods

    #region IHostApplicationBuilder

    private static AppInfo common(object builder, AppInfo? appInfo = null, AppInfoOptions? options = null)
    {
        appInfo ??= new(); // ENH: Create from GetDefaultAppInfo
        options ??= new();

        if (options.AutoDetect) appInfo.AutoDetect();
      

        var appDirectories = new AppDirectories(appInfo);

        #region AppInfo Singletons

        if (LionFireEnvironment.IsMultiApplicationEnvironment) // REVIEW: eliminate this in favor of DependencyContext.Current?
        {
            if (Applications.AppInfo.RootInstance == null)
            {
                Applications.AppInfo.RootInstance = appInfo;
            }
        }

        #endregion

        void configureServices(IServiceCollection s)
        {
            s
               .AddSingleton(appInfo)
               .AddSingleton(appDirectories)
               ;
        }

        if (builder is IHostApplicationBuilder hab)
        {
            hab.Properties[typeof(AppInfo)] = appInfo;
            hab.Configuration.AddAppInfo(appInfo);
            configureServices(hab.Services);
        }
        else if (builder is IHostBuilder hb)
        {
            hb.Properties[typeof(AppInfo)] = appInfo;
            hb.ConfigureHostConfiguration(cb => cb.AddAppInfo(appInfo));
            hb.ConfigureServices(configureServices);
        }
        else { throw new NotImplementedException(); }

        if (options.AutoCreateDirectories) appDirectories.AutoCreateDirectories(appInfo);

        return appInfo;
    }
    public static T AppInfo<T>(this T builder, AppInfo? appInfo = null, AppInfoOptions? options = null)
        where T : IHostApplicationBuilder
    {
        common(builder, appInfo, options);
        return builder;
    }

    #endregion

    #region IHostBuilder

    public static AppInfo? AppInfo(this IHostBuilder builder)
        => builder.Properties[typeof(AppInfo)] as AppInfo;
    //public static AppInfo AppInfo(this IHostBuilder hostBuilder)
    //    => hostBuilder.Properties[typeof(AppInfo)] as AppInfo ?? throw new ArgumentException("IHostBuilder needs to have AddAppInfo() invoked on it.");

    public static IHostBuilder AppInfo(this IHostBuilder builder, AppInfo? appInfo = null, AppInfoOptions? options = null)
    {
        common(builder, appInfo, options);
        return builder;
    }

    #endregion

    public static ILionFireHostBuilder AppInfo(this ILionFireHostBuilder builder, AppInfo appInfo, AppInfoOptions? options = null)
    {
        if (builder.HostBuilder.WrappedIHostApplicationBuilder != null) { builder.HostBuilder.WrappedIHostApplicationBuilder.AppInfo(appInfo, options); }
        else if (builder.HostBuilder.WrappedIHostApplicationBuilder != null) { builder.HostBuilder.WrappedIHostApplicationBuilder.AppInfo(appInfo, options); }
        else { throw new NotImplementedException(); }
        //builder.HostBuilder.AppInfo(appInfo, options: options);
        return builder;
    }

    #endregion
}

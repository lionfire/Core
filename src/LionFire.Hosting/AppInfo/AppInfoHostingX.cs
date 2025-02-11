using LionFire.Applications;
using LionFire.ExtensionMethods.Copying;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class AppInfoHostingX
{

    #region IServiceCollection

    /// <summary>
    /// Prefer AppInfo on ILionFireHostBuilder if available.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appInfo"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <remarks>
    /// Private because options 
    /// </remarks>
    private static IServiceCollection AddAppInfo(this IServiceCollection services, AppInfo appInfo, AppInfoOptions? options = null)
    {
        var appDirectories = new AppDirectories(appInfo);

        services
                .Configure<AppInfo>(a => a.AssignPropertiesFrom(appInfo))
               .AddSingleton(sp => sp.GetRequiredService<IOptions<AppInfo>>().Value)
               //.AddTransient(sp => sp.GetRequiredService<IOptionsSnapshot<AppInfo>>().Value)
               .AddSingleton(appDirectories)
               ;

        Task.Run(() =>
        {
            options ??= new();
            if (options.AutoCreateDirectories) appDirectories.AutoCreateDirectories(appInfo);
        });

        return services;
    }

    #endregion
    #region Host Builder extension methods

    #region IHostApplicationBuilder

    /// <summary>
    /// - Invokes IServiceCollection.AddAppInfo
    /// - Adds configuration: AppConfigurationKeys.*
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="appInfo"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private static AppInfo common(object builder, AppInfo? appInfo = null, AppInfoOptions? options = null)
    {
        appInfo ??= new(); // ENH: Create from GetDefaultAppInfo
        options ??= new();
        if (options.AutoDetect) appInfo.AutoDetect();

        #region (optional) AppInfo.RootInstance Singleton

        if (LionFireEnvironment.IsMultiApplicationEnvironment) // REVIEW: eliminate this in favor of DependencyContext.Current?
        {
            if (Applications.AppInfo.RootInstance == null)
            {
                Applications.AppInfo.RootInstance = appInfo; // First one becomes the RootInstance
            }
        }

        #endregion

        if (builder is IHostApplicationBuilder hab)
        {
            //hab.Properties[typeof(AppInfo)] = appInfo; // OLD
            hab.Configuration.AddAppInfo(appInfo);
            hab.Services.AddAppInfo(appInfo, options);
        }
        else if (builder is IHostBuilder hb)
        {
            //hb.Properties[typeof(AppInfo)] = appInfo; // OLD
            hb.ConfigureHostConfiguration(cb => cb.AddAppInfo(appInfo));
            hb.ConfigureServices(s => s.AddAppInfo(appInfo, options));
        }
        else { throw new NotImplementedException(); }

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

    public static IHostBuilder AppInfo(this IHostBuilder builder, AppInfo? appInfo = null, AppInfoOptions? options = null)
    {
        common(builder, appInfo, options);
        return builder;
    }

    #endregion

    public static ILionFireHostBuilder AppInfo(this ILionFireHostBuilder builder, AppInfo appInfo, AppInfoOptions? options = null)
    {
        if (builder.HostBuilder.WrappedIHostApplicationBuilder != null) { builder.HostBuilder.WrappedIHostApplicationBuilder.AppInfo(appInfo, options); }
        else if (builder.HostBuilder.WrappedHostBuilder != null) { builder.HostBuilder.WrappedHostBuilder.AppInfo(appInfo, options); }
        else { throw new NotImplementedException(); }
        return builder;
    }

    #endregion

    #region AppInfo Accessors before Build

    // Previously, AppInfo was available on Properties, but now it is only available as a service.

    //public static AppInfo? AppInfo(this IHostBuilder builder) => builder.Properties[typeof(AppInfo)] as AppInfo; // OLD (avoids building)
    public static AppInfo GetAppInfo(this IHostBuilder hostBuilder)
        => throw new Exception("UNTESTED - verify this works");
    //hostBuilder.Properties[typeof(AppInfo)] as AppInfo ?? throw new ArgumentException("IHostBuilder needs to have AddAppInfo() invoked on it.");

    public static AppInfo GetAppInfo(this IHostApplicationBuilder builder)
      => builder.Services.BuildServiceProvider().GetService<AppInfo>() ?? throw new ArgumentException("HostApplicationBuilder needs to have AddAppInfo() already invoked on it.");

    #endregion

    public static IServiceCollection DataDir(this IServiceCollection services, string dataDirName)
        => services.Configure<AppInfo>(a => a.DataDirName = dataDirName);
    public static ILionFireHostBuilder DataDir(this ILionFireHostBuilder lf, string dataDirName)
        => lf.ConfigureServices(s => s.Configure<AppInfo>(a => a.DataDirName = dataDirName));


}

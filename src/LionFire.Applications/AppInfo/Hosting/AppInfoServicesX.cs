#nullable enable
using LionFire.Applications;
using LionFire.Configuration;
using LionFire.Dependencies;
using LionFire.ExtensionMethods.Dumping;
using LionFire.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LionFire.Hosting;

public static class AppInfoServicesX
{

    public static AppInfo AutoDetect(this AppInfo appInfo)
    {
        appInfo ??= new();

        if (string.IsNullOrEmpty(appInfo.OrgNamespace)) { appInfo.OrgNamespace ??= ApplicationAutoDetection.AutoDetectOrgNamespace(null); }
        appInfo.OrgName ??= appInfo.OrgNamespace;
        appInfo.OrgDisplayName ??= appInfo.OrgName;
        appInfo.AppName ??= ApplicationAutoDetection.AutoDetectAppName(null, appInfo.OrgName);
        appInfo.AppDir ??= appInfo.AutodetectedAppDir;

        return appInfo;
    }

    public static IConfigurationBuilder AddAppInfo(this IConfigurationBuilder configurationBuilder, AppInfo appInfo)
        => configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [AppConfigurationKeys.OrgName] = appInfo.OrgName,
            [AppConfigurationKeys.AppName] = appInfo.AppName,
            [AppConfigurationKeys.AppVersion] = appInfo.ProgramVersion,
            [AppConfigurationKeys.DataDirName] = appInfo.DataDirName,
        });

    #region Host Builder extension methods

    #region HostApplicationBuilder
        
    public static HostApplicationBuilder AppInfo(this HostApplicationBuilder builder, AppInfo? appInfo = null, AppInfoOptions? options = null)
    {
        //Action<AppInfo>? config = null,
        options ??= new();
        appInfo ??= new();

        //if (config != null) config(appInfo);
        if (options.AutoDetect) appInfo.AutoDetect();
        builder.GetLogger(typeof(AppInfo).FullName!).LogInformation($"AppInfo: {appInfo.Dump()}");

        builder.Services.AddSingleton(appInfo);

        #region AppInfo Singletons

        if (LionFireEnvironment.IsMultiApplicationEnvironment) // REVIEW: eliminate this in favor of DependencyContext.Current?
        {
            if (Applications.AppInfo.RootInstance == null)
            {
                Applications.AppInfo.RootInstance = appInfo;
            }
        }
        //DependencyContext.Current?.SetType<AppInfo>(appInfo); // ENH: Set on Flex context?

        // ENH: Alternate idea to reduce coupling: publish a 'new ServiceAvailable<T>(appInfo)' message

        #endregion

        #region Missing functionality

        //builder.AsHostBuilder().Properties[typeof(AppInfo)] = appInfo; // Unavailable due to internal access modifier

        #endregion

        #region HostApplicationBuilder only

        builder.Configuration.AddAppInfo(appInfo);

        #endregion

        if (options.AutoCreateDirectories) AutoCreateDirectories(appInfo); // BLOCKING I/O

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
        //Action<AppInfo>? config = null,
        options ??= new();
        appInfo ??= new();

        //if (config != null) config(appInfo);
        if (options.AutoDetect) appInfo.AutoDetect();
        builder.GetLogger(typeof(AppInfo).FullName!).LogInformation($"AppInfo: {appInfo.Dump()}");

        builder.ConfigureServices(s => s.AddSingleton(appInfo));

        if (LionFireEnvironment.IsMultiApplicationEnvironment)
        {
            if (Applications.AppInfo.RootInstance == null)
            {
                Applications.AppInfo.RootInstance = appInfo;
            }
        }

        if (options.AutoCreateDirectories) AutoCreateDirectories(appInfo);

        builder.Properties[typeof(AppInfo)] = appInfo;

        #region Adapted to IHostBuilder

        builder.ConfigureHostConfiguration(cb => cb.AddAppInfo(appInfo));

        #endregion



        return builder;
    }

    #endregion

    public static ILionFireHostBuilder AppInfo(this ILionFireHostBuilder builder, AppInfo appInfo, AppInfoOptions? options = null)
    {
        builder.HostBuilder.AppInfo(appInfo, options: options);
        return builder;
    }


    #endregion


    public static void AutoCreateDirectories(AppInfo appInfo)
    {
        DirectoryUtils.EnsureAllDirectoriesExist(typeof(LionFireEnvironment.Directories));
        //DirectoryUtils.EnsureAllDirectoriesExist<AppDirectories>();
        AppDirectories.CreateProgramDataFolders(appInfo);
    }


}

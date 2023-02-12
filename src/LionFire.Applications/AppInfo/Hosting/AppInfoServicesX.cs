#nullable enable
using LionFire.Applications;
using LionFire.Configuration;
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
            [AppConfigurationKeys.DataDirName] = appInfo.DataDirName,
        });

    #region Host Builder extension methods


    public static HostApplicationBuilder AppInfo(this HostApplicationBuilder builder, AppInfo? appInfo = null, Action<AppInfo>? config = null, AppInfoOptions? options = null)
    {
        options ??= new();
        appInfo ??= new();

        if (config != null) config(appInfo);
        if (options.AutoDetect) appInfo.AutoDetect();
        builder.GetLogger(typeof(AppInfo).FullName).LogInformation($"AppInfo: {appInfo.Dump()}");

        builder.Services.AddSingleton(appInfo);
        builder.Configuration.AddAppInfo(appInfo);

        if (LionFireEnvironment.IsMultiApplicationEnvironment)
        {
            if (Applications.AppInfo.RootInstance == null)
            {
                Applications.AppInfo.RootInstance = appInfo;
            }
        }

        if (options.AutoCreateDirectories) AutoCreateDirectories(appInfo);

        return builder;
    }

    public static void AutoCreateDirectories(AppInfo appInfo)
    {
        DirectoryUtils.EnsureAllDirectoriesExist(typeof(LionFireEnvironment.Directories));
        //DirectoryUtils.EnsureAllDirectoriesExist<AppDirectories>();
        AppDirectories.CreateProgramDataFolders(appInfo);
    }

    // Note: Not using Configure approach (cooperative construction) because this is needed early
    public static IHostBuilder AppInfo(this IHostBuilder builder, AppInfo appInfo = null, bool autoDetect = true, bool autoCreateDirectories = false)
    {
        appInfo ??= new AppInfo();
        if (autoDetect) appInfo.AutoDetect();
        builder.GetLogger(typeof(AppInfo).FullName).LogInformation($"AppInfo: {appInfo.Dump()}");

        #region Enable ambient access to AppInfo

        //hostBuilder.Properties[typeof(AppInfo)] = appInfo; // DEPRECATED - Is this still needed?

        builder.ConfigureHostConfiguration(c => c.AddAppInfo(appInfo));

        if (LionFireEnvironment.IsMultiApplicationEnvironment)
        {
            if (Applications.AppInfo.RootInstance == null)
            {
                Applications.AppInfo.RootInstance = appInfo;
            }
        }
        //else // OLD
        //{
        //    Applications.AppInfo.Instance = appInfo;
        //}

        #endregion

        if (autoCreateDirectories) AutoCreateDirectories(appInfo);

        builder.ConfigureServices(services => services.AddSingleton(appInfo));
        return builder;
    }

    #endregion

}

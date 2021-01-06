using LionFire.Applications;
using LionFire.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LionFire.Hosting
{
    public static class ApplicationInfoServicesExtensions
    {
        public static IHostBuilder AddAppInfo(this IHostBuilder hostBuilder, AppInfo appInfo = null, bool autoDetect = true, bool autoCreateDirectories = false)
        {
            if (appInfo == null) appInfo = new AppInfo();

            if (autoDetect)
            {
                if (appInfo.OrgName == null) appInfo.OrgName = ApplicationAutoDetection.AutoDetectOrganizationName(null);
                if (appInfo.AppName == null) appInfo.AppName = ApplicationAutoDetection.AutoDetectApplicationName(null);
            }

            hostBuilder.Properties[typeof(AppInfo)] = appInfo;

            hostBuilder.ConfigureHostConfiguration(c =>
            {
                c.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["OrgName"] = appInfo.OrgName,
                    ["AppName"] = appInfo.AppName,
                    ["DataDirName"] = appInfo.DataDirName,
                });
            });
            hostBuilder.ConfigureServices((context, services) => services.AddSingleton(appInfo));

            if(!LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                AppInfo.Instance = appInfo;
            }
            else
            {
                if (AppInfo.RootInstance == null)
                {
                    AppInfo.RootInstance = appInfo;
                }
            }

            appInfo.AppDir = appInfo.AutodetectedAppDir;
            hostBuilder.GetLogger(typeof(AppInfo).FullName).LogInformation($"AppDir: {appInfo.AutodetectedAppDir}");

            if(autoCreateDirectories)
            {
                DirectoryUtils.EnsureAllDirectoriesExist(typeof(LionFireEnvironment.Directories));
                //DirectoryUtils.EnsureAllDirectoriesExist<AppDirectories>();
                AppDirectories.CreateProgramDataFolders(appInfo);
            }
            return hostBuilder;
        }
    }
}

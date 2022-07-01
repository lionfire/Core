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
        // Note: Not using Configure approach (cooperative construction) because this is needed early

        public static IHostBuilder AppInfo(this IHostBuilder hostBuilder, AppInfo appInfo = null, bool autoDetect = true, bool autoCreateDirectories = false)
        {
            if (appInfo == null) appInfo = new AppInfo();

            #region AutoDetect

            if (autoDetect)
            {
                if (appInfo.OrgName == null) appInfo.OrgName = ApplicationAutoDetection.AutoDetectOrganizationName(null);
                if (appInfo.AppName == null) appInfo.AppName = ApplicationAutoDetection.AutoDetectApplicationName(null);
            }
            appInfo.AppDir = appInfo.AutodetectedAppDir;
            hostBuilder.GetLogger(typeof(AppInfo).FullName).LogInformation($"AppDir: {appInfo.AutodetectedAppDir}");
            
            #endregion

            #region Enable ambient access to AppInfo

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
            
            if(!LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                Applications.AppInfo.Instance = appInfo;
            }
            else
            {
                if (Applications.AppInfo.RootInstance == null)
                {
                    Applications.AppInfo.RootInstance = appInfo;
                }
            }

            #endregion


            #region Initialization

            if (autoCreateDirectories)
            {
                DirectoryUtils.EnsureAllDirectoriesExist(typeof(LionFireEnvironment.Directories));
                //DirectoryUtils.EnsureAllDirectoriesExist<AppDirectories>();
                AppDirectories.CreateProgramDataFolders(appInfo);
            }
            
            #endregion

            hostBuilder.ConfigureServices((context, services) => services.AddSingleton(appInfo));
            return hostBuilder;
        }
    }
}

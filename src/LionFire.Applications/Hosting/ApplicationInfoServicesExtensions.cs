using LionFire.Applications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace LionFire.Services
{
    public static class ApplicationInfoServicesExtensions
    {


        public static IHostBuilder SetAppInfo(this IHostBuilder hostBuilder, AppInfo appInfo = null, bool autoDetect = true)
        {
            if (appInfo == null) appInfo = new AppInfo();

            if (autoDetect)
            {
                if (appInfo.OrgName == null) appInfo.OrgName = ApplicationAutoDetection.AutoDetectOrganizationName(null);
                if (appInfo.AppName == null) appInfo.AppName = ApplicationAutoDetection.AutoDetectApplicationName(null);
            }

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

            hostBuilder.Properties.Add("AppInfo", appInfo);

            return hostBuilder;
        }
    }
}

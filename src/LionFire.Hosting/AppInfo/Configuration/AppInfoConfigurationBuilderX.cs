#nullable enable
using LionFire.Applications;
using LionFire.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace LionFire.Hosting;

public static class AppInfoConfigurationBuilderX
{
    public static IConfigurationBuilder AddAppInfo(this IConfigurationBuilder configurationBuilder, AppInfo appInfo)
        => configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [AppConfigurationKeys.OrgName] = appInfo.OrgName,
            [AppConfigurationKeys.AppName] = appInfo.AppName,
            [AppConfigurationKeys.AppVersion] = appInfo.ProgramVersion,
            [AppConfigurationKeys.DataDirName] = appInfo.DataDirName,
        });
}

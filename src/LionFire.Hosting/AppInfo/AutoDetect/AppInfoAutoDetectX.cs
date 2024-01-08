#nullable enable
using LionFire.Applications;

namespace LionFire.Hosting;

public static class AppInfoAutoDetectX
{
    public static AppInfo AutoDetect(this AppInfo? appInfo)
    {
        appInfo ??= new();

        appInfo.AutoDetectMissingValues();

        return appInfo;
    }

    // REVIEW - There's a new Default creation mechanism: GetDefaultAppInfo.  Merge the two?
    public static AppInfo? AutoDetectMissingValues(this AppInfo appInfo)
    {
        if (string.IsNullOrEmpty(appInfo.OrgNamespace)) { appInfo.OrgNamespace ??= ApplicationAutoDetection.AutoDetectOrgNamespace(null); }
        appInfo.OrgName ??= appInfo.OrgNamespace;
        appInfo.OrgDisplayName ??= appInfo.OrgName;
        appInfo.AppName ??= ApplicationAutoDetection.AutoDetectAppName(null, appInfo.OrgName);
        appInfo.AppDir ??= appInfo.AutodetectedAppDir;

        return appInfo;
    }

}

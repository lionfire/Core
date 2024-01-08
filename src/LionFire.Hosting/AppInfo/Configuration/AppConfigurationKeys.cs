
using Microsoft.Extensions.Hosting;

namespace LionFire.Configuration;

public static class AppConfigurationKeys
{
    public const string OrgName = "OrgName";
    public static string AppName => HostDefaults.ApplicationKey; // "applicationName"
    public const string DataDirName = "DataDirName"; // REVIEW - See also: HostDefaults.ContentRootKey
    public const string AppVersion = "AppVersion";
}

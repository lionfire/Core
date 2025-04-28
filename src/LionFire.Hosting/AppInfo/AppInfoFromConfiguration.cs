#nullable enable
using System;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using LionFire.Hosting;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

// REVIEW: Replace/merge AppInfo to prefer this?

public partial class AppInfoFromConfiguration // RENAME to be simpler
{
    public IConfiguration configuration { get; }
    public ILogger<AppInfoFromConfiguration> Logger { get; }

    public AppInfoFromConfiguration(IConfiguration configuration, ILogger<AppInfoFromConfiguration> logger)
    {
        this.configuration = configuration;
        Logger = logger;
    }
    public string? ApplicationName() => configuration?[HostDefaults.ApplicationKey];
    public string? Environment() => configuration?[HostDefaults.EnvironmentKey];
    public string? ContentRoot() => configuration?[HostDefaults.ContentRootKey];

    public void DumpToLog()
    {
        Logger.LogInformation("AppInfoFromConfiguration.ApplicationName {ApplicationName}", ApplicationName(configuration));
        Logger.LogInformation("AppInfoFromConfiguration.ApplicationNameOrFallback {ApplicationNameOrFallback}", ApplicationNameOrFallback(configuration));
        Logger.LogInformation("Environment: {Environment}", Environment(configuration));
        Logger.LogInformation("ContentRoot: {ContentRoot}", ContentRoot(configuration));
        Logger.LogInformation("ReleaseChannel: {ReleaseChannel}", configuration[Deployment.ReleaseChannelKeys.ReleaseChannel]);
    }
}

public partial class AppInfoFromConfiguration // Static
{
    public static string? ApplicationName(IConfiguration? configuration)
        => configuration?[HostDefaults.ApplicationKey];
    public static string? Environment(IConfiguration? configuration)
        => configuration?[HostDefaults.EnvironmentKey];
    public static string? ContentRoot(IConfiguration? configuration)
        => configuration?[HostDefaults.ContentRootKey];

    public static string ApplicationNameFallback
    {
        get
        {
            if (LionFireEnvironment.IsUnitTest == true)
            {
                for (int i = 0; i < 9999; i++)
                {
                    var f = new StackFrame(i);
                    var mi = f.GetMethod();
                    if (mi == null) break;
                    if(mi.GetCustomAttributes().Where(a => a.GetType().Name == "TestMethodAttribute").Any())
                    {
                        return mi.DeclaringType + "." + mi.Name;
                    }
                }
            }

            return Assembly.GetEntryAssembly()?.GetName().Name + (LionFireEnvironment.IsUnitTest == true ? "-" + Guid.NewGuid().ToString() : "") ?? Guid.NewGuid().ToString();
        }
    }

    public static string? TestDllName
    {
        get
        {
            if (LionFireEnvironment.IsUnitTest == true)
            {
                for (int i = 0; i < 9999; i++)
                {
                    var f = new StackFrame(i);
                    var mi = f.GetMethod();
                    if (mi == null) break;
                    if (mi.GetCustomAttributes().Where(a => a.GetType().Name == "TestMethodAttribute").Any() && mi.DeclaringType != null)
                    {
                        return Path.GetFileNameWithoutExtension(mi.DeclaringType.Assembly.Location);
                    }
                }
            }
            return null;
        }
    }

    public static string ApplicationNameOrFallback(IConfiguration? configuration)
        => ApplicationName(configuration) ?? ApplicationNameFallback;
}

using LionFire.Applications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class AppInfoDefaultsX
{
    public static ILionFireHostBuilder TrySetDefaultAppInfo(this ILionFireHostBuilder lf)
    {
        var appInfo = lf.GetDefaultAppInfo();

        if (appInfo != null) { lf.AppInfo(appInfo); }

        return lf;
    }

    public static AppInfo? GetDefaultAppInfo(this ILionFireHostBuilder lf)
    {
        IHostEnvironment? env = lf.IHostApplicationBuilder?.Environment; // no IHostBuilder support
        return env.GetDefaultAppInfo();
    }

    /// <summary>
    /// Uses IHostEnvironment.ApplicationName to determine the Org and AppName.  
    /// 
    /// ApplicationName should be in the form of "Org.AppName" or "Org.AppName.SubAppName" etc.
    /// 
    /// Also initializes domain name if preconfigured in RecognizedNamespaces.
    /// </summary>
    /// <param name="env"></param>
    /// <returns></returns>
    public static AppInfo? GetDefaultAppInfo(this IHostEnvironment? env)
    {
        AppInfo? appInfo;

        if (env == null) return null;

        var indexOfFirstDot = env.ApplicationName.IndexOf('.');
        if (indexOfFirstDot <= 0) return null;

        var org = env.ApplicationName.Substring(0, indexOfFirstDot);

        if (!RecognizedNamespaces.TryGetValue(org, out var recognized)) return null;

        var orgName = recognized.orgNameOverride ?? org;

        var appName = env.ApplicationName;
        if(appName.StartsWith(orgName + ".")) { appName = appName.Substring(orgName.Length + 1); }
        appInfo = new AppInfo(appName, orgName, recognized.orgDomain)
        {
            OrgDomain = recognized.orgDomain
        };

        return appInfo;
    }

    /// <summary>
    /// Take IHostEnvironment.ApplicationName, and use the segment before the first '.' as the key here.
    /// </summary>
    public static Dictionary<string, (string orgDomain, string? orgNameOverride)> RecognizedNamespaces = new Dictionary<string, (string orgDomain, string? orgNameOverride)>
    {
        // HARDCODE: First-party Defaults  (MOVE to LionFire.Internal)
        ["LionFire"] = ("lionfire.ca", null),
        ["Valor"] = ("lionfire.ca", "LionFire"),
        ["FireLynx"] = ("firelynx.io", null),
    };

    //public static AppInfo GetAppInfo(this IServiceCollection services)
    //{
    //    return services.BuildServiceProvider().GetRequiredService<IOptions<AppInfo>>().Value;
    //}
}
//public class AppInfoOptionsFactory : OptionsFactory<AppInfo>
//{
//    public AppInfoOptionsFactory(IEnumerable<IConfigureOptions<AppInfo>> setups, IEnumerable<IPostConfigureOptions<AppInfo>> postConfigures) : base(setups, postConfigures)
//    {
//    }

//    public AppInfoOptionsFactory(IEnumerable<IConfigureOptions<AppInfo>> setups, IEnumerable<IPostConfigureOptions<AppInfo>> postConfigures, IEnumerable<IValidateOptions<AppInfo>> validations) : base(setups, postConfigures, validations)
//    {
//    }

//    protected override AppInfo CreateInstance(string name)
//    {
//        return AppInfo.Default;
//        return base.CreateInstance(name);
//    }
//}

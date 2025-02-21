using LionFire.Dependencies;
using LionFire.Structures;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LionFire.Applications;

public class AppInfo
{
    #region Static

#if OLD
    [Obsolete("Use DependencyContext.Current.GetService<AppInfo>()")]
    public static AppInfo Default => @default ??= new AppInfo(); // REVIEW - AutoDetectMissingValues ?  - get from an ambient IServiceProvider?
    private static AppInfo? @default;

    /// <summary>
    /// See also: ServiceLocator.Get&lt;AppInfo&gt;()
    /// </summary>
    [Obsolete("See DependencyContext.Current.Get<AppInfo>()")]
    public static AppInfo Instance
    {
        get => DependencyContext.Current.GetRequiredService<AppInfo>();
        //get => ManualSingleton<AppInfo>.Instance;
        //set
        //{
        //    if (Instance != null) throw new AlreadySetException();
        //    if (LionFireEnvironment.IsMultiApplicationEnvironment)
        //    {
        //        throw new NotSupportedException("Cannot set AppInfo.Instance when LionFireEnvironment.IsMultiApplicationEnvironment is true.  Use RootInstance instead.");
        //    }
        //    ManualSingleton<AppInfo>.Instance = value;
        //}
    }
#endif

    public static AppInfo? RootInstance
    {
        get => rootInstance;
        set
        {
            if (rootInstance != null) throw new AlreadySetException();
            rootInstance = value;
        }
    }
    private static AppInfo? rootInstance;

    #endregion

    #region Construction

    public AppInfo()
    {
    }

    public AppInfo(string appName, string orgName, string? orgDomain = null) : this()
    {
        AppName = appName;
        OrgName = orgName;
        OrgNamespace = OrgName;
        OrgDomain = orgDomain ?? DefaultOrgDomain;
    }

    #endregion

    public bool IsValid =>
        OrgDomain != DefaultOrgDomain;

    public string? InvalidReason
    {
        get
        {
            if (OrgDomain == DefaultOrgDomain) { return "OrgDomain must be set (cannot be DefaultOrgDomain)"; }
            return null;
        }
    }

    public string FullAppId => OrgDomain.Split('.').Reverse().Concat(AppId.Split('.')).Aggregate((x, y) => $"{x}.{y}");

    #region Identity Properties

    /// <summary>
    /// Recommended: Globally unique, having your organization in the name.
    /// Recommended: FullName of your program type, or your program's namespace with the program name appended.
    /// Alternate: Reverse domain name style
    /// Default: $"{OrgName}.{AppName.Replace(" ", "")}"
    /// </summary>
    public string? AppId // Rename AppDomainUrl
    {
        get => appId ?? DefaultAppId;
        set => appId = value;
    }
    private string? appId;

    public string? AppIdReverse // Rename AppReverseDomainUrl
    {
        get => appIdReverse ?? DefaultAppIdReverse;
        set => appIdReverse = value;
    }
    private string? appIdReverse;

    public const string AppIdSchemePrefix = "app:";

    protected string? DefaultAppIdReverse => AppIdSchemePrefix + DefaultAppDomainReverse;
    protected string? DefaultAppId => AppIdSchemePrefix + DefaultAppKey;
    //{
    //get 
    //        {
    //            if (defaultAppId == null && AppName != null)
    //            {
    //#if false // reverse
    //                if (OrgDomain != null)
    //                {
    //                    defaultAppId = OrgDomain.Split('.').Reverse().Aggregate((x, y) => $"{x}.{y}") + "." + AppName;
    //                }
    //                else if(OrgName != null)
    //                {
    //                    return appId ?? $"{OrgName}.{AppName.Replace(" ", "")}";
    //                }
    //#else
    //                if (OrgDomain != null)
    //                {
    //                    defaultAppId = AppIdSchemePrefix + AppName + "." + OrgDomain;
    //                }
    //                else if (OrgName != null)
    //                {
    //                    return appId ?? (AppIdSchemePrefix + $"{AppName.Replace(" ", "")}.{OrgName}");
    //                }
    //#endif
    //            }
    //            return defaultAppId;
    //        }
    //}
    //private string? defaultAppId;

    public static string? ReverseDomain(string? domain) => domain?.Split('.').Reverse().Aggregate((x, y) => $"{x}.{y}");

    public string? AppDomainReverse
    {
        get => appDomainReverse ?? DefaultAppDomainReverse;
        set => appDomainReverse = value;
    }
    private string? appDomainReverse;

    protected string? DefaultAppDomain
    {
        get
        {
            if (defaultAppDomain == null && AppNamespace != null)
            {
                if (OrgDomain != null) { defaultAppDomain = (ReverseDomain(AppName) + "." + OrgDomain).ToLowerInvariant(); } // Kebab case?
                else if (AppNamespace != null) { defaultAppDomain = ReverseDomain(AppNamespace); }
                else if (OrgName != null) { defaultAppDomain = ReverseDomain(AppNamespace) ?? ($"{ReverseDomain(AppName)}.{OrgName}"); }
            }
            return defaultAppDomain;
        }
    }
    private string? defaultAppDomain;

    protected string? DefaultAppDomainReverse //=> ReverseDomain(DefaultAppKey);
    {
        get
        {
            if (defaultAppDomainReverse == null && AppNamespace != null)
            {
                if (OrgDomain != null) { defaultAppDomainReverse = (ReverseDomain(OrgDomain) + "." + AppName).ToLowerInvariant(); } // Kebab case?
                else if (AppNamespace != null) { defaultAppDomainReverse = AppNamespace; }
                else if (OrgName != null) { defaultAppDomainReverse = AppNamespace ?? ($"{OrgName}.{AppName}"); }
            }
            return defaultAppDomainReverse;
        }
    }
    private string? defaultAppDomainReverse;

    /// <summary>
    /// Default: $"{AppName}.{OrgDomain}"
    /// Fallback: $"{AppName}.{OrgName}"
    /// 
    /// </summary>
    protected string? DefaultAppKey
    {
        get
        {
            return defaultAppKey ??= DefaultAppDomain;

            //            if (defaultAppKey == null && AppName != null)
            //            {
            //#if false // reverse
            //                if (OrgDomain != null)
            //                {
            //                    defaultAppKey = OrgDomain.Split('.').Reverse().Aggregate((x, y) => $"{x}.{y}") + "." + AppName;
            //                }
            //                else if(OrgName != null)
            //                {
            //                    return appKey ?? $"{OrgName}.{AppName.Replace(" ", "")}";
            //                }
            //#else
            //                if (OrgDomain != null) { defaultAppKey = DefaultAppDomain; }
            //                else if (OrgName != null) { return appName ?? ($"{AppName.Replace(" ", "")}.{OrgName}"); }
            //#endif
            //}
            //return defaultAppKey;
        }
    }
    private string? defaultAppKey;

    public string OrgDomain { get; set; } = DefaultOrgDomain;
    public static string DefaultOrgDomain = "example.com";

    /// <summary>
    /// Typically the same as OrgName, but could be multi-part
    /// </summary>
    public string OrgNamespace { get; set; } = "";

    /// <summary>
    /// Recommendation: no spaces
    /// TODO: Rename this to OrgId (typically it will match first chunk of namespace in C# code), and have a separate OrgName that can contain spaces
    /// </summary>
    public string? OrgName { get; set; }
    public string? OrgDisplayName { get; set; }

    #endregion

    #region Directory names

    public string? OrgDir => orgDir ?? OrgName;
    private string? orgDir;

    #endregion

    /// <summary>
    /// Partial Id of an app within an Org, should be without OrgNamespace
    /// 
    /// Recommendations: 
    ///  - unique name within your organization
    ///  - no spaces, unless your executable file has spaces in it.  (Spaces may be removed when deriving a URL or domain.)
    /// </summary>
    /// <remarks>
    /// Intended to match:
    ///  - IHostBuilder.Properties[HostDefaults.ApplicationKey]
    ///  - Configuration[HostDefaults.ApplicationKey]
    ///  - note: HostDefaults.ApplicationKey = "AppName"
    /// </remarks>
    public string? AppName
    {
        get => appName;
        set => appName = value;
    }
    private string? appName;

    public string? AppNamespace => appNamespace ??= (OrgNamespace + "." + appName?.Replace(" ", ""))?.TrimStart('.');
    private string? appNamespace;

    public string? DevProjectName
    {
        get => devProjectName ?? AppName;
        set => devProjectName = value;
    }
    private string? devProjectName;

    public string? AppDisplayName
    {
        get => appDisplayName ?? AppName;
        set => appDisplayName = value;
    }
    private string? appDisplayName;

    public string? AppLongDisplayName
    {
        get => appLongDisplayName ?? AppName;
        set => appLongDisplayName = value;
    }
    private string? appLongDisplayName;


    // FUTURE: Allow multiple data dirs
    public string? DataDirName { get; set; }
    public string? EffectiveDataDirName => DataDirName ?? AppName?.Replace('.', Path.DirectorySeparatorChar);

    public string ProgramVersion { get; set; } = "0.0.0";

#if OLD
    [Obsolete]
    public string VersionString => ProgramVersion;
#endif

    #region Directories

    //public AppDirectories Directories => DependencyContext.Current.GetService<AppDirectories>();
    //{
    //    get
    //    {
    //        if (directories == null)
    //        {
    //            directories = new AppDirectories(this);
    //            directories.Initialize();
    //        }
    //        return directories;
    //    }
    //    set => directories = value;
    //}
    //private AppDirectories directories;

    #endregion

    public string CurrentDirectory => Environment.CurrentDirectory;

    #region AppDir

    public string? AppDir
    {
        get => appDir;
        set => appDir = value;
    }
    private string? appDir;

    public string? AutodetectedAppDir => autodetectedAppDir ??= DetectAppDir();
    private string? autodetectedAppDir;

    public string DetectAppDir()
    {
        AppInfo appInfo = this;
        var result = LionFireEnvironment.ExeDir;

        string releaseEnding = @"bin\release";
        string debugEnding = @"bin\debug";
        //string debugEnding2 = @"dbin"; // UNUSED but maybe bring it back
        string binEnding = @"bin";

        string releaseProjectEnding = @"bin\" + appInfo.DevProjectName.ToLowerInvariant() + @"\release";
        string debugProjectEnding = @"bin\" + appInfo.DevProjectName.ToLowerInvariant() + @"\debug";

        if (result.ToLower().EndsWith(releaseEnding))
        {
            result = result.Substring(0, result.Length - releaseEnding.Length);
        }
        else if (result.ToLower().EndsWith(debugEnding))
        {
            result = result.Substring(0, result.Length - debugEnding.Length);
        }
        //else if (result.ToLower().EndsWith(debugEnding2)) // UNUSED
        //{
        //    result = result.Substring(0, result.Length - debugEnding2.Length);
        //}
        else if (result.ToLower().EndsWith(binEnding))
        {
            result = result.Substring(0, result.Length - binEnding.Length);
        }
        else if (result.ToLower().EndsWith(releaseProjectEnding))
        {
            result = Path.Combine(result.Substring(0, result.Length - releaseProjectEnding.Length), appInfo.AppName);
        }
        else if (result.ToLower().EndsWith(debugProjectEnding))
        {
            result = Path.Combine(result.Substring(0, result.Length - debugProjectEnding.Length), appInfo.AppName);
        }
        else
        {
            Debug.WriteLine("Could not determine AppDir.  Using AppDir = ExeDir: " + LionFireEnvironment.ExeDir);
            result = LionFireEnvironment.ExeDir;
        }
        return result;
    }
    #endregion

    /// <summary>
    /// Custom directory name for the application.  Example: c:\ProgramData\{OrgDir}\{CustomAppDir}
    /// </summary>
    public string? CustomAppDirName { get; set; }

}

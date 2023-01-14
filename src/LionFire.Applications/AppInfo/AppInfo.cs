using LionFire.Dependencies;
using LionFire.Structures;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LionFire.Applications;

public class AppInfo
{
    #region Static

    internal static AppInfo Default = new AppInfo();

    /// <summary>
    /// See also: ServiceLocator.Get&lt;AppInfo&gt;()
    /// </summary>
    [Obsolete("See DependencyContext.Current.Get<AppInfo>()")]
    public static AppInfo Instance
    {
        get => DependencyContext.Current.GetService<AppInfo>();
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

    public static AppInfo RootInstance
    {
        get => rootInstance;
        set
        {
            if (rootInstance != null) throw new AlreadySetException();
            rootInstance = value;
        }
    }
    private static AppInfo rootInstance;

    #endregion

    #region Construction

    public AppInfo()
    {
    }

    public AppInfo(string appName, string orgName)
    {
        AppName = appName;
        OrgName = orgName;
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
    public string AppId
    {
        get => appId ?? DefaultAppId;
        set => appId = value;
    }
    private string appId;

    protected string DefaultAppId
    {
        get
        {
            if (defaultAppId == null && AppName != null)
            {
                if (OrgDomain != null)
                {
                    defaultAppId = OrgDomain.Split('.').Reverse().Aggregate((x, y) => $"{x}.{y}") + "." + AppName;
                }
                else if(OrgName != null)
                {
                    return appId ?? $"{OrgName}.{AppName.Replace(" ", "")}";
                }
            }
            return defaultAppId;
        }
    }
    private string defaultAppId;

    public string OrgDomain { get; set; } = DefaultOrgDomain;
    public static string DefaultOrgDomain = "example.com";
    public string OrgNamespace { get; set; } = "Example";

    /// <summary>
    /// Recommendation: no spaces
    /// </summary>
    public string? OrgName { get; set; }

    #endregion

    #region Directory names

    public string OrgDir => orgDir ?? OrgName;
    private string orgDir;

    #endregion

    /// <summary>
    /// Recommendations: 
    ///  - unique name within your organization
    ///  - no spaces, unless your executable file has spaces in it.
    /// </summary>
    public string AppName
    {
        get => appName ?? appId;
        set => appName = value;
    }
    private string appName;

    public string DevProjectName
    {
        get => devProjectName ?? AppName;
        set => devProjectName = value;
    }
    private string devProjectName;

    public string AppDisplayName
    {
        get => appDisplayName ?? AppName;
        set => appDisplayName = value;
    }

    private string appDisplayName;
    public string AppLongDisplayName
    {
        get => appLongDisplayName ?? AppName;
        set => appLongDisplayName = value;
    }
    private string appLongDisplayName;


    // FUTURE: Allow multiple data dirs
    public string DataDirName { get; set; }
    public string EffectiveDataDirName => DataDirName ?? AppName;

    public string ProgramVersion { get; set; } = "0.0.0";
    [Obsolete]
    public string VersionString => ProgramVersion; 

    #region Directories

    public AppDirectories Directories
    {
        get
        {
            if(directories == null)
            {
                directories = new AppDirectories(this);
                directories.Initialize();
            }
            return directories;
        }
        set => directories = value;
    }
    private AppDirectories directories;

    #endregion


    #region AppDir

    public string AppDir
    {
        get => appDir;
        set => appDir = value;
    }
    private string appDir;

    public string AutodetectedAppDir => autodetectedAppDir ??= DetectAppDir();
    private string autodetectedAppDir;

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
    public string CustomAppDirName { get; set; }

}

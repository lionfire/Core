using LionFire.Dependencies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.Applications;

// REVIEW: Reconsider all this in light of .NET Core's new default classes for this sort of thing

public static class ApplicationEnvironment
{
    #region AppInfo

#if UNUSED
    [Obsolete("Use AppInfoFromContext or DependencyContext.Current.GetService<AppInfo>()")]
    public static AppInfo StaticAppInfo
    {
        get => appInfo;
        set
        {
            if (appInfo == value) { return; }

            /*if (mainAppInfo != default(AppInfo))
            {
                throw new Exception("Already set");
            }*/

            appInfo = value;
        }
    }
    private static AppInfo appInfo;
    public static bool IsMainAppInfoSet => appInfo != null;
    public static AppInfo DefaultAppInfo => AppInfoFromContext ?? AppInfo.Default;
#endif

    public static AppInfo AppInfoFromContext => DependencyContext.Current.GetService<AppInfo>();

    #endregion

#if UNUSED
    #region Convenience
    public static string OrgName => StaticAppInfo?.OrgName; 
    public static string ProgramName => StaticAppInfo?.AppName;
    public static string ProgramDisplayName => StaticAppInfo?.AppDisplayName;
    #endregion
#endif

    #region MachineGuid

    public static Guid MachineGuid
    {
        get
        {
            if (machineGuid == null)
            {
                var path = Path.Combine(DependencyContext.Current.GetRequiredService<AppDirectories>().CompanyProgramData, "machineid.txt");
                string guidString;
                Guid guid;
                if (File.Exists(path))
                {
                    guidString = File.ReadAllText(path);
                    if (Guid.TryParse(guidString, out guid))
                    {
                        machineGuid = guid;
                    }
                    else
                    {
                        throw new Exception("Machine GUID file is corrupt.  Please delete or restore it: " + path);
                    }
                }
                else
                {
                    guid = Guid.NewGuid();
                    File.WriteAllText(path, guid.ToString());
                    machineGuid = guid;
                }
            }
            return machineGuid.Value;
        }
    }
    private static Guid? machineGuid;

    #endregion
}

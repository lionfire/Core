using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.Applications
{
    public static class ApplicationEnvironment
    {
        #region MainAppInfo

        public static AppInfo AppInfo
        {
            get => mainAppInfo ?? AppInfo.Default;
            set
            {
                if (mainAppInfo == value)
                {
                    return;
                }

                /*if (mainAppInfo != default(AppInfo))
                {
                    throw new Exception("Already set");
                }*/

                mainAppInfo = value;
            }
        }
        private static AppInfo mainAppInfo;
        public static bool IsMainAppInfoSet => mainAppInfo != null;

        #endregion

        #region Convenience

        public static string OrgName => AppInfo?.OrgName; 
        public static string ProgramName => AppInfo?.AppName;
        public static string ProgramDisplayName => AppInfo?.ProgramDisplayName;

        #endregion

        #region MachineGuid

        public static Guid MachineGuid
        {
            get
            {
                if (machineGuid == null)
                {
                    var path = Path.Combine(AppInfo.Directories.CompanyProgramData, "machineid.txt");
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
                return machineGuid;
            }
        }
        private static Guid machineGuid;

        #endregion
    }
}

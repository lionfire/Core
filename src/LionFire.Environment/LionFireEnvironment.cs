using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Structures;
using Microsoft.Extensions.PlatformAbstractions;

namespace LionFire
{
    public class AppInfo
    {
        internal static AppInfo Default = new AppInfo();
        public string CompanyName { get; set; } = "LionFireUser";
        public string ProgramName { get; set; } = "ProgramName";
        public string ProgramDisplayName { get; set; } = "Program Name";

        public string CustomAppDataDirName { get; set; }

        public string ProgramVersion { get; set; } = "0.0.0";
    }

    public partial class LionFireEnvironment
    {
        #region App-definable settings

        #region MainAppInfo

        public static AppInfo MainAppInfo
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

        public static string CompanyName => MainAppInfo?.CompanyName;
        public static string ProgramName => MainAppInfo?.ProgramName;
        public static string ProgramDisplayName => MainAppInfo?.ProgramDisplayName;

        #endregion

        #region Compile Environment

        /// <summary>
        /// .NET Framework:	NET20, NET35, NET40, NET45, NET451, NET452, NET46, NET461, NET462, NET47, NET471, NET472
        /// .NET Standard:   NETSTANDARD1_0, NETSTANDARD1_1, NETSTANDARD1_2, NETSTANDARD1_3, NETSTANDARD1_4, NETSTANDARD1_5, NETSTANDARD1_6, NETSTANDARD2_0
        /// .NET Core:   NETCOREAPP1_0, NETCOREAPP1_1, NETCOREAPP2_0, NETCOREAPP2_1
        /// - https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries
        /// </summary>
        public static string CompileTargetFrameworkMoniker =>
#if NET451
                    return "NET451";
#elif NET461
                    return "net461";
#elif NET462
                    return "net462";
#elif NET471
                    return "net471";
#elif NET472
                    return "net472";
#elif NETCOREAPP1_0
                return "NETCOREAPP1_0";
#elif NETSTANDARD2_0
                "NETSTANDARD2_0";
#elif NETCOREAPP2_0
                return "NETCOREAPP2_0";
#elif NETCOREAPP2_1
                return "NETCOREAPP2_1";
#else
                throw new NotImplementedException("TODO: Implement - https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries");
#endif


        #endregion

        #region Runtime Environment

        //PlatformServices.Default.Application.RuntimeFramework

        #endregion

        #region Streams

        public static TextWriter StandardOutput { get; private set; }

        public static Stream StandardOutputStream
        {
            get
            {
                if (standardOutputStream == null) { StandardOutputStream = Console.OpenStandardOutput(); }
                return standardOutputStream;
            }
            set
            {
                standardOutputStream = value;
                StandardOutput = new StreamWriter(value);
            }
        }
        private static Stream standardOutputStream;

        #endregion

        #region MachineName

        public static string MachineName
        {
            get => machineName ?? System.Environment.MachineName;
            set => machineName = value;
        }
        private static string machineName;

        #endregion

        #region MachineGuid

        public static Guid MachineGuid
        {
            get
            {
                if (machineGuid == null)
                {
                    var path = Path.Combine(Directories.CompanyProgramData, "machineid.txt");
                    string guidString;
                    Guid guid;
                    bool parsed;
                    if (File.Exists(path))
                    {
                        guidString = File.ReadAllText(path);
                        if (parsed = Guid.TryParse(guidString, out guid))
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

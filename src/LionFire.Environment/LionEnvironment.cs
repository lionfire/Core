using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using LionFire.ExtensionMethods;


namespace LionFire
{

    public partial class LionFireEnvironment
    {
        #region Compile Environment

        public static string CompileTargetFrameworkMoniker
        {
            get
            {
#if NET451
                           return "NET451";
#elif NET461
                    return "net461";
#elif NET462
                    return "net462";
#elif NETCOREAPP1_0
                return "NETCOREAPP1_0";
#else
                throw new NotImplementedException();
#endif
            }
        }

        #endregion

        #region Runtime Environment

        //PlatformServices.Default.Application.RuntimeFramework

        #endregion

        #region Program Environment

        public static string HomeDir
        {
            get
            {
                return Environment.ExpandEnvironmentVariables("%HOMEPATH%");
            }
        }

        // %LOCALAPPDATA% LOCALAPPDATA

        // C:\Users\{username}\AppData\Roaming
        public static string AppDataDir
        {
            get
            {
                if (appDataDir == null)
                {
                    var result = Environment.ExpandEnvironmentVariables("%APPDATA%");
                    if (String.IsNullOrWhiteSpace(result))
                    {
                        throw new Exception("%APPDATA% is not set");
                    }
                }
                return appDataDir;
            }
            set { appDataDir = value; }
        }
        private static string appDataDir;

        //	C:\ProgramData
        public static string ProgramDataDir
        {
            get
            {
                if (programDataDir == null)
                {
                    programDataDir = Environment.ExpandEnvironmentVariables("%ALLUSERSPROFILE%");
                    if (String.IsNullOrWhiteSpace(programDataDir))
                    {
                        throw new Exception("%ALLUSERSPROFILE% is not set");
                    }
                }
                return programDataDir;
            }
            set { programDataDir = value; }
        }
        private static string programDataDir;

        public static string CompanyProgramDataDir
        {
            get
            {
                if (String.IsNullOrWhiteSpace(CompanyName)) return null;
                return Path.Combine(ProgramDataDir, CompanyName);
            }
        }

        // c:\ProgramData\{CompanyName}\{namedDataDir}
        public static string GetProgramDataDir(string namedDataDir)
        {
            return Path.Combine(ProgramDataDir, CompanyName, namedDataDir);
        }

        public static string CompanyName { get; set; } = "LionFire";
        public static string ProgramName { get; set; } = "ProgramName";
        public static string ProgramDisplayName { get; set; } = "Program Name";

        // RENAME ProgramDataDirName.  Create an AppDataDirName somewhere inside AppHost
        public static string AppDataDirName
        {
            get
            {
                return appDataDirName ?? ProgramName;
            }
            set { appDataDirName = value; }
        }
        private static string appDataDirName;

        // c:\ProgramData\{CompanyName}\{AppDataDirName}
        public static string AppProgramDataDir
        {
            get { return GetProgramDataDir(AppDataDirName); }
        }

        //[SetOnce]
        public static string ProgramVersion { get; set; } = "0.0.0";

        #endregion

        public static TextWriter StandardOutput { get; private set; }
        
        public static Stream StandardOutputStream
        {
            get {
                if (standardOutputStream == null) { StandardOutputStream = Console.OpenStandardOutput(); }
                return standardOutputStream; }
            set
            {
                standardOutputStream = value;
                StandardOutput = new StreamWriter(value);
            }
        }
        private static Stream standardOutputStream;

        #region Construction

        static LionFireEnvironment()
        {
            Directories.EnsureAllDirectoriesExist();            
        }

        #endregion

    }
}

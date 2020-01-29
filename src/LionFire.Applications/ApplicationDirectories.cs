using System;
using System.Diagnostics;
using System.IO;

namespace LionFire.Applications
{
    public static class ApplicationDirectories
    {
        #region (Parameterized) Custom Data Dir in ProgramData

        /// <summary>
        /// c:\ProgramData\{CompanyName}\{namedDataDir}
        /// </summary>
        /// <param name="namedDataDir"></param>
        /// <returns></returns>
        public static string GetProgramDataDir(string namedDataDir) => Path.Combine(LionFireEnvironment.Directories.ProgramData, CompanyName, namedDataDir);

        #endregion


        /// <summary>
        /// c:\ProgramData\{CompanyName}\{AppDataDirName}
        /// </summary>
        public static string AppProgramDataDir => GetProgramDataDir(AppDataDirName);

        #region App data dir name.  Change to use a known dir that does not match App name.

        /// <summary>
        /// Dir name under the company name.   If not set, ProgramName will be used.
        /// 
        /// MOVE - to within AppInfo
        /// RENAME ProgramDataDirName.  Create an AppDataDirName somewhere inside AppHost
        /// </summary>
        public static string AppDataDirName
        {
            get => ApplicationEnvironment.AppInfo?.DataDirName ?? ApplicationEnvironment.AppInfo.AppName;
            set => appDataDirName = value;
        }
        private static string appDataDirName;

        // REFACTOR: Merge VosAppHost ExeDir finding logic into here
        public static string AppBinDir
        {
            get
            {
#if UNITY
                //				return UnityEngine.Application.dataPath;
                return PersistentDataPath;
#else

#if OLD // Doesn't work with obfuscators
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                if(assembly == null) return "\"";
                
                return System.IO.Path.GetDirectoryName(assembly.Location); 
#else
                var p = Process.GetCurrentProcess();
                var result = p.MainModule.FileName;

                //l.Debug("Process.GetCurrentProcess().MainModule.ModuleName - " + p.MainModule.ModuleName);
                //l.Debug("Process.GetCurrentProcess().MainModule.FileName - " + p.MainModule.FileName);
                //l.Debug("Process.GetCurrentProcess().StartInfo.FileName - " + p.StartInfo.FileName); // Usually empty
                //l.Debug("SEnvironment.CommandLine - " + SEnvironment.CommandLine);

                return System.IO.Path.GetDirectoryName(result);
#endif
#endif
            }
        }

        // TODO: Move VosAppHost AppDir finding logic (looking for application.json) into here
        public static string AppDir
        {
            get
            {
#if UNITY
                return UnityEngine.Application.dataPath;
                //				return UnityEngine.Application.persistentDataPath;
#else
                if (appDir == null)
                {
                    appDir = AppBinDir;

                    string releaseEnding = @"bin\release";
                    string debugEnding = @"bin\debug";
                    string debugEnding2 = @"dbin";
                    string binEnding = @"bin";

                    string releaseProjectEnding = @"bin\" + ApplicationEnvironment.AppInfo.AppName.ToLowerInvariant() + @"\release";
                    string debugProjectEnding = @"bin\" + ApplicationEnvironment.AppInfo.AppName.ToLowerInvariant() + @"\debug";

                    if (appDir.ToLower().EndsWith(releaseEnding))
                    {
                        appDir = appDir.Substring(0, appDir.Length - releaseEnding.Length);
                    }
                    else if (appDir.ToLower().EndsWith(debugEnding))
                    {
                        appDir = appDir.Substring(0, appDir.Length - debugEnding.Length);
                    }
                    else if (appDir.ToLower().EndsWith(debugEnding2))
                    {
                        appDir = appDir.Substring(0, appDir.Length - debugEnding2.Length);
                    }
                    else if (appDir.ToLower().EndsWith(binEnding))
                    {
                        appDir = appDir.Substring(0, appDir.Length - binEnding.Length);
                    }
                    else if (appDir.ToLower().EndsWith(releaseProjectEnding))
                    {
                        appDir = Path.Combine(appDir.Substring(0, appDir.Length - releaseProjectEnding.Length), ApplicationEnvironment.AppInfo.AppName);
                    }
                    else if (appDir.ToLower().EndsWith(debugProjectEnding))
                    {
                        appDir = Path.Combine(appDir.Substring(0, appDir.Length - debugProjectEnding.Length), ApplicationEnvironment.AppInfo.AppName);
                    }
                    else
                    {
                        Debug.WriteLine("Could not determine AppDir.  Using AppDir = AppBinDir: " + AppBinDir);
                        appDir = AppBinDir;
                    }
                    //l.Info("AppDir: " + appDir);
                }
                return appDir;
#endif
            }
        }
        private static string appDir;


        #endregion
        private static string CompanyName => ApplicationEnvironment.AppInfo.OrgName;

        #region Company

        /// <summary>
        /// C:\ProgramData\{CompanyName}
        /// </summary>
        public static string CompanyProgramData
        {
            get
            {
                if (String.IsNullOrWhiteSpace(CompanyName)) return null;
                return Path.Combine(LionFireEnvironment.Directories.ProgramData, CompanyName);
            }
        }

        [Obsolete("Use CompanyLocalAppDataPath")]
        public static string CompanyUserFolderPath => CompanyLocalAppDataPath;
        public static string CompanyLocalAppDataPath => Path.Combine(LionFireEnvironment.Directories.LocalAppData, CompanyName);
        //            {
        //                get
        //                {
        //                    return Path.Combine(LocalAppData, CompanyName);
        ////#if UNITY
        ////                var path = Path.Combine(SEnvironment.GetFolderPath(SEnvironment.SpecialFolder.LocalApplicationData), LionSEnvironment.CompanyName);
        ////#else
        ////                    var path = Path.Combine(SEnvironment.GetFolderPath(SEnvironment.SpecialFolder.LocalApplicationData, SEnvironment.SpecialFolderOption.Create), LionSEnvironment.CompanyName);
        ////#endif
        ////                    EnsureCreated(path);
        //                    //return path;
        //                }
        //            }

        #endregion

        #region App

        //public static string DefaultDataRoot
        //{
        //    get
        //    {
        //        return defaultDataDir ?? AppDir;
        //    }
        //    set
        //    {
        //        // MISSINGDEPENDENCY - AlreadyException, also somewhere else in this file.
        //        if (defaultDataDir != default(string)) throw new Exception("Cannot be changed after setting to a non-null value.");
        //        defaultDataDir = value;
        //    }
        //}
        //private static string defaultDataDir;

        #endregion

        //#region Logging

        //public static string LogsDir
        //{
        //    get
        //    {
        //        var assembly = Assembly.GetEntryAssembly();
        //        if (CompanyProgramData == null || assembly == null) return null;
        //        return Path.Combine(CompanyProgramData, "Logs", assembly.GetName().Name);
        //    }
        //}

        //#endregion

    }
}

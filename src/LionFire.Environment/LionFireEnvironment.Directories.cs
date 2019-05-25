using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SEnvironment = System.Environment;

namespace LionFire
{
    public partial class LionFireEnvironment
    {
        // .NET Framework Special Folders:
        //  - http://stackoverflow.com/questions/895723/environment-getfolderpath-commonapplicationdata-is-still-returning-c-docum

        public static bool AutoCreateDirectories = true;

        public static class Directories
        {
            #region Construction

            static Directories()
            {
                if (AutoCreateDirectories) { EnsureAllDirectoriesExist(); }
            }

            #endregion

            public static Dictionary<string, string> Other { get; } = new Dictionary<string, string>();

            #region OS

            /// <summary>
            /// C:\ProgramData
            /// </summary>
            public static string ProgramData
            {
                get
                {
                    if (programData == null)
                    {
                        //SEnvironment.GetFolderPath(SEnvironment.SpecialFolder.CommonApplicationData)
                        programData = SEnvironment.ExpandEnvironmentVariables("%ALLUSERSPROFILE%");
                        if (String.IsNullOrWhiteSpace(programData))
                        {
                            throw new Exception("%ALLUSERSPROFILE% is not set");
                        }
                    }
                    return programData;
                }
                set { programData = value; }
            }
            private static string programData;

            /// <summary>
            /// C:\Users\{username}\AppData\Roaming
            /// </summary>
            public static string RoamingAppData
            {
                get
                {
                    if (appData == null)
                    {
                        var result = SEnvironment.ExpandEnvironmentVariables("%APPDATA%");
                        if (String.IsNullOrWhiteSpace(result))
                        {
                            throw new Exception("%APPDATA% is not set");
                        }
                    }
                    return appData;
                }
                set { appData = value; }
            }
            private static string appData;

            /// <summary>
            /// C:\Users\{username}\AppData\Local
            /// </summary>
            public static string LocalAppData // Rename LocalAppDataDir
            {
                get
                {
                    if (localAppData == null)
                    {
                        var result = SEnvironment.ExpandEnvironmentVariables("%LOCALAPPDATA%");
                        if (String.IsNullOrWhiteSpace(result))
                        {
                            throw new Exception("%LOCALAPPDATA% is not set");
                        }
                    }
                    return localAppData;
                }
                set { localAppData = value; }
            }
            private static string localAppData;
            
            /// <summary>
            /// \\Users\{username}  (missing drive letter?)
            /// </summary>
            public static string HomeDir
            {
                get
                {
                    return SEnvironment.ExpandEnvironmentVariables("%HOMEPATH%");
                }
            }

            #endregion

            #region Company

            /// <summary>
            /// C:\ProgramData\{CompanyName}
            /// </summary>
            public static string CompanyProgramData
            {
                get
                {
                    if (String.IsNullOrWhiteSpace(CompanyName)) return null;
                    return Path.Combine(ProgramData, CompanyName);
                }
            }

            [Obsolete("Use CompanyLocalAppDataPath")]
            public static string CompanyUserFolderPath => CompanyLocalAppDataPath;
            public static string CompanyLocalAppDataPath=> Path.Combine(LocalAppData, CompanyName);
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

            #region (Parameterized) Custom Data Dir in ProgramData

            /// <summary>
            /// c:\ProgramData\{CompanyName}\{namedDataDir}
            /// </summary>
            /// <param name="namedDataDir"></param>
            /// <returns></returns>
            public static string GetProgramDataDir(string namedDataDir)
            {
                return Path.Combine(ProgramData, CompanyName, namedDataDir);
            }

            #endregion

            #region App data dir name.  Change to use a known dir that does not match App name.

            /// <summary>
            /// Dir name under the company name.   If not set, ProgramName will be used.
            /// 
            /// MOVE - to within AppInfo
            /// RENAME ProgramDataDirName.  Create an AppDataDirName somewhere inside AppHost
            /// </summary>
            public static string AppDataDirName
            {
                get
                {
                    return MainAppInfo?.CustomAppDataDirName ?? ProgramName;
                }
                set { appDataDirName = value; }
            }
            private static string appDataDirName;

            #endregion

            #region App
            
            /// <summary>
            /// c:\ProgramData\{CompanyName}\{AppDataDirName}
            /// </summary>
            public static string AppProgramDataDir
            {
                get { return GetProgramDataDir(AppDataDirName); }
            }

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

                        string releaseProjectEnding = @"bin\" + MainAppInfo.ProgramName.ToLowerInvariant() + @"\release";
                        string debugProjectEnding = @"bin\" + MainAppInfo.ProgramName.ToLowerInvariant() + @"\debug";

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
                            appDir = Path.Combine(appDir.Substring(0, appDir.Length - releaseProjectEnding.Length), MainAppInfo.ProgramName);
                        }
                        else if (appDir.ToLower().EndsWith(debugProjectEnding))
                        {
                            appDir = Path.Combine(appDir.Substring(0, appDir.Length - debugProjectEnding.Length), MainAppInfo.ProgramName);
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

            #region User

            public static string UserProfile
            {
                get
                {
#if NET462 || NET461 || NET451
                    return SEnvironment.GetFolderPath(SEnvironment.SpecialFolder.UserProfile);
#else
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        return SEnvironment.GetEnvironmentVariable("USERPROFILE");
                    }
                    else
                    {
                        return SEnvironment.GetEnvironmentVariable("HOME");
                    }
#endif

                }
            }

            #endregion

            #region Logging

            public static string LogsDir
            {
                get
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (CompanyProgramData == null || assembly == null) return null;
                    return Path.Combine(CompanyProgramData, "Logs", assembly.GetName().Name);
                }
            }

            #endregion

            #region (Private) Utility Methods

            internal static void EnsureAllDirectoriesExist()
            {
                foreach (var pi in typeof(LionFireEnvironment.Directories).GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    try
                    {
                        var dir = (pi.GetValue(pi) as string);
                        dir.EnsureDirectoryExists();
                    }
                    catch
                    {
                        // EMPTYCATCH TODO
                    }
                }
            }

            #endregion


            #region FUTURE

            #region Windows Store

            // Windows store:
            // http://stackoverflow.com/a/21274767/208304
            // Windows.Storage.ApplicationData.Current.LocalFolder or Windows.Storage.ApplicationData.Current.RoamingFolder

            #endregion

            #endregion

        }
    }
}

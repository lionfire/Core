#if OLD
using LionFire.Dependencies;
using System;
using System.Diagnostics;
using System.IO;

namespace LionFire.Applications
{
    public static class AppDirectoriesHelpers
    {
        //private static AppInfo appInfo => ServiceLocator.Get<AppInfo>();
        //private static AppDirectories appDirectories => ServiceLocator.Get<AppInfo>()?.Directories;

        #region Company


        //[Obsolete("Use CompanyLocalAppDataPath")]
        //public static string CompanyUserFolderPath => CompanyLocalAppDataPath;
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
#endif

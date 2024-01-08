﻿//#define ENABLE_APPDATA_FALLBACK
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LionFire.Applications;
using LionFire.Dependencies;

namespace LionFire.Vos.VosApp
{

    public static class VosDiskPaths
    {
        private static AppInfo AppInfo => DependencyContext.Current.GetService<AppInfo>();
        private static AppDirectories AppDirectories => DependencyContext.Current.GetService<AppDirectories>();

        public static string Base => @"Base";
        //public static string BasePacks { get { return @"Static\BasePacks"; } }
        public static string Data => "Data";
        //public static string Packs { get { return "Packs"; } }
        public static string Database { get { return "DBs"; } }

        #region AppDir

        public static string AppRoot => AppDirectories.AppDir;

        public static string AppBase => Path.Combine(AppRoot, VosDiskPaths.Base);

        //public static string AppBasePacks
        //{
        //    get { return Path.Combine(AppRoot, VosDiskPaths.BasePacks); }
        //}

        public static string AppData
        {
            get { return Path.Combine(AppRoot, VosDiskPaths.Data); }
        }
        //public static string AppPacks
        //{
        //    get { return Path.Combine(AppRoot, VosDiskPaths.Packs); }
        //}

        #endregion

        #region AppData

#if ENABLE_APPDATA_FALLBACK

            public static string BaseDataPath_AppData
            {
                get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), VosDiskPaths.BaseData); }
            }

            public static string BasePacksPath_AppData
            {
                get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), VosDiskPaths.BaseDataPacks); }
            }


            public static string UserDataPath_AppData
            {
                get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), VosDiskPaths.UserData); }
            }

            public static string UserPacksPath_AppData
            {
                get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), VosDiskPaths.UserDataPacks); }
            }

#endif

        #endregion

        #region Shared

#if !UNITY
        public static string GlobalSharedDataRoot => Path.Combine(AppDirectories.CompanyProgramData, "Data");
        public static string GlobalSharedStoresRoot => Path.Combine(AppDirectories.CompanyProgramData, "Stores");
#endif

        public static string UserSharedDataRoot => Path.Combine(AppDirectories.CompanyLocalAppDataPath, "Data");
        public static string UserSharedStoresRoot => Path.Combine(AppDirectories.CompanyLocalAppDataPath, "Stores");

        #endregion

        #region Var (CommonAppData aka Var aka ProgramData)

        public static string VarRoot { get { return AppDirectories.AppProgramDataDir; } } // c:\ProgramData\{CompanyFolder}\{applicationFolder}

        public static string VarBase
        {
            get { return varBase ?? Path.Combine(VarRoot, VosDiskPaths.Base); }
            set { varBase = value; }
        } private static string varBase;

        //public static string VarBasePacks
        //{
        //    get { return Path.Combine(VarRoot, VosDiskPaths.BasePacks); }
        //}

        public static string VarData
        {
            get { return varData?? Path.Combine(VarRoot, VosDiskPaths.Data); }
            set
            {
                varData = value;
            }
        } private static string varData = null;

        //public static string VarPacks
        //{
        //    get { return Path.Combine(VarRoot, VosDiskPaths.Packs); }
        //}

        #endregion

        public static string CompanyVarDir(string companyName = null)
        {
            if (companyName == null) { companyName = ApplicationEnvironment.OrgName; }
            return Path.Combine(VarRoot, companyName);
        }

        public static string AppVarDir(string appName = null, string companyName = null)
        {
            if (appName == null) { appName = AppInfo.EffectiveDataDirName;  }
            if (companyName == null) { companyName = ApplicationEnvironment.OrgName; }

            return Path.Combine(CompanyVarDir(companyName), appName);
        }
        public static string AppVarDataDir(string appName = null, string companyName = null)
        {
            if (appName == null) { appName = AppInfo.EffectiveDataDirName; }
            if (companyName == null) { companyName = ApplicationEnvironment.OrgName; }
            return Path.Combine(AppVarDir(appName, companyName), VosDiskPaths.Data);
        }

        #region Default

        /// <summary>
        /// Defaults to null.
        /// </summary>
        public static string CustomRoot { get; set; }

        public static string CustomBase => Path.Combine(CustomRoot, VosDiskPaths.Base);

        //public static string CustomBasePacks
        //{
        //    get { return Path.Combine(CustomRoot, VosDiskPaths.BasePacks); }
        //}

        public static string CustomData => Path.Combine(CustomRoot, VosDiskPaths.Data);

        //public static string CustomDataPacks
        //{
        //    get { return Path.Combine(CustomRoot, VosDiskPaths.Packs); }
        //}

        #endregion

        //public static class Databases
        //{
        //    public static string Default
        //    {
        //        get { return VosDiskPaths.Default.Databases; }
        //    }
        //}

        //public static class Default
        //{
        //    public static string Databases
        //    {
        //        get { return Path.Combine(LionEnvironment.DefaultDataRoot, VosDiskPaths.Database); }
        //    }
        //}
    }

}

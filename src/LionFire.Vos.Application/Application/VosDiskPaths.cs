//#define ENABLE_APPDATA_FALLBACK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LionFire.ObjectBus
{

    public static class VosStoreNames
    {
        public const string AppBase = "AppBase";
        public const string AppData = "AppData";
        public const string VarBase = "VarBase";
        public const string VarData = "VarData";
        public const string CustomBase = "CustomBase";
        public const string CustomData = "CustomData";

        [Obsolete]
        internal static readonly string AppDirLayerName = "ProgramFiles";
        [Obsolete]
        internal static readonly string CommonAppDataLayerName = "CommonAppData";

        //internal static readonly string AppDataLayerName = "AppData";

        //internal static readonly string DataDbLayerName = "DataDb";

    }

    public static class VosDiskPaths
    {
        public static string Base { get { return @"Base"; } }
        //public static string BasePacks { get { return @"Static\BasePacks"; } }
        public static string Data { get { return "Data"; } }
        //public static string Packs { get { return "Packs"; } }
        public static string Database { get { return "DBs"; } }

        #region AppDir

        public static string AppRoot { get { return LionEnvironment.AppDir; } }

        public static string AppBase
        {
            get { return Path.Combine(AppRoot, VosDiskPaths.Base); }
        }
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
        public static string GlobalSharedDataRoot { get { return Path.Combine(LionEnvironment.CompanyFolderPath, "Data"); } }
        public static string GlobalSharedStoresRoot { get { return Path.Combine(LionEnvironment.CompanyFolderPath, "Stores"); } }
#endif

        public static string UserSharedDataRoot { get { return Path.Combine(LionEnvironment.CompanyUserFolderPath, "Data"); } }
        public static string UserSharedStoresRoot { get { return Path.Combine(LionEnvironment.CompanyUserFolderPath, "Stores"); } }

        #endregion

        #region Var (CommonAppData aka Var aka ProgramData)

        public static string VarRoot { get { return LionEnvironment.CommonApplicationFolderPath; } }

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
            if (companyName == null) { companyName = LionEnvironment.CompanyName; }
            return Path.Combine(LionEnvironment.VarRoot, companyName);
        }

        public static string AppVarDir(string appName = null, string companyName = null)
        {
            if (appName == null) { appName = LionEnvironment.CommonAppDirDirectoryName;  }
            if (companyName == null) { companyName = LionEnvironment.CompanyName; }

            return Path.Combine(CompanyVarDir(companyName), appName);
        }
        public static string AppVarDataDir(string appName = null, string companyName = null)
        {
            if (appName == null) { appName = LionEnvironment.CommonAppDirDirectoryName; }
            if (companyName == null) { companyName = LionEnvironment.CompanyName; }
            return Path.Combine(AppVarDir(appName, companyName), VosDiskPaths.Data);
        }

        #region Default

        /// <summary>
        /// Defaults to null.
        /// </summary>
        public static string CustomRoot { get; set; }

        public static string CustomBase
        {
            get { return Path.Combine(CustomRoot, VosDiskPaths.Base); }
        }

        //public static string CustomBasePacks
        //{
        //    get { return Path.Combine(CustomRoot, VosDiskPaths.BasePacks); }
        //}

        public static string CustomData
        {
            get { return Path.Combine(CustomRoot, VosDiskPaths.Data); }
        }

        //public static string CustomDataPacks
        //{
        //    get { return Path.Combine(CustomRoot, VosDiskPaths.Packs); }
        //}

        #endregion

        public static class Databases
        {
            public static string Default
            {
                get { return VosDiskPaths.Default.Databases; }
            }
        }

        public static class Default
        {
            public static string Databases
            {
                get { return Path.Combine(LionEnvironment.DefaultDataRoot, VosDiskPaths.Database); }
            }
        }
    }

}

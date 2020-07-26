#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Vos.VosApp
{
    // TODO REVIEW - are all of these still relevant?


    public static class VosPaths
    {
        public const string App = "/App"; // TODO: I am going with `.  Alias it?
        public const string ActiveDataPath = App + "/~";
        public const string Settings = App + "/.settings";

        public const string RootPath = "/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Path to named or primary root, with trailing slash.</returns>
        public static string GetRootPath(string? name)
        {
            if(string.IsNullOrEmpty(name)) { return RootPath; }
            return $"/../{name}/";
        }

        #region AppInternals


        public const string AppInternals = App + "/_"; // REVIEW - Rename to Internals or VosApp Internals

        public const string PackageMounts = AppInternals + "/PackageMounts";

        public const string Packages = AppInternals + "/Packages";

        public const string PackageStores = AppInternals + "/PackageStores";

        public const string Stores = AppInternals + "/Stores";
        public const string StoreMounts = AppInternals + "/StoreMounts";

        #endregion

#if NEVER
        public const string UserPath = AppPath + "/User";
#endif

        //public const string BaseDataPath = App + "/Base";

        public const string LocalPath = App + "/Local";
        public const string AccountsPath = LocalPath + "/Accounts";

        public const string DBsPath = App + "/DBs";

        public static string MetaDataSubPath { get { return "._"; } } // TODO - Change this as it's not allowed in Windows Explorer UI
        public static string MetaDataSubPathPrefix { get { return "._/"; } }
    }

    // MOVE
    public static class DycenVosPaths
    {
        public const string ModulesPath = VosPaths.AppInternals + "/Modules";
        public const string ModulesAvailablePath = VosPaths.AppInternals + "/ModulesAvailable";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public static class VosPaths
    {
        public const string App = "/App";
        public const string ActiveDataPath = App + "/$";

        public const string AppInternals = App + "/_";

        public const string PackageMounts = AppInternals + "/PackageMounts";

        public const string Packages = AppInternals + "/Packages";

        public const string PackageStores = AppInternals + "/PackageStores";

        public const string Stores = AppInternals + "/Stores";
        public const string StoreMounts = AppInternals + "/StoreMounts";

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

    public static class DycenVosPaths
    {
        public const string ModulesPath = VosPaths.AppInternals + "/Modules";
        public const string ModulesAvailablePath = VosPaths.AppInternals + "/ModulesAvailable";
    }
}

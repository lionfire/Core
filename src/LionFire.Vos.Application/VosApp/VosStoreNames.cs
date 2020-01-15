//#define ENABLE_APPDATA_FALLBACK
using System;

namespace LionFire.Vos.VosApp
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

}

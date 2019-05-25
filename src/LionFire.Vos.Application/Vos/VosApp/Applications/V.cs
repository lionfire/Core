//#define ASSETS_SUBPATH // Prefer this off?  TODO - make sure this works for packages

using System;
using LionFire.Vos;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{
    public static class V
    {
        public static Vob Root
        {
            get { return VosApp.Instance.Root; }
        }

        public static bool HasActiveData { get { return VosApp.Instance != null && VosApp.Instance.HasActiveData; } }

        /// <summary>
        /// The primary place to store application data.  Multiple locations may be mounted at this point to create an overlay effect.  Data that is written typically goes to the OS-specific location for variable program data (such as /var or C:\ProgramData\MyApp)
        /// </summary>
        public static Vob ActiveData
        {
            get
            {
                if (VosApp.Instance == null)
                {
                    if (l != null) l.Error("VosApp.Instance == null " + Environment.StackTrace);
                    return null;
                }
                return VosApp.Instance.ActiveData;
            }
        }
        public static Vob Assets
        {
            get { return VosApp.Instance.Assets; }
        }

        /// <summary>
        /// Typically mounted at application install dir, subpath "Data".  Not recommended as a write destination, except for add-on packs.
        /// </summary>
        public static Vob AppData
        {
            get { return VosApp.Instance.AppData; }
        }

        public static Vob Packages
        {
            get { return VosApp.Instance.Packages; }
        }
        public static Vob Archives
        {
            get { return VosApp.Instance.PackageStores; }
        }
        public static Vob Stores
        {
            get { return VosApp.Instance.Stores; }
        }

        private static ILogger l = Log.Get();
    }
}

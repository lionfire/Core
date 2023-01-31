//#define ASSETS_SUBPATH // Prefer this off?  TODO - make sure this works for packages
using System;
using LionFire.Dependencies;
using LionFire.Vos;
using LionFire.Vos.Packages;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos.VosApp
{
    /// <summary>
    /// Convenient way to access current VobApp, which is a convenient way to access Vobs at conventional paths.  
    /// See VosApp class for documentation on properties.
    /// </summary>
    public static class V
    {
        // TODO: Cache values of properties

        public static IRootVob Root => DependencyContext.Current.GetService<IVos>().Get();

        #region Accessor

        public static VosDirs VosApp
        {
            get => vosApp ?? VosAppGetter();
            set => vosApp = value;
        }
        private static VosDirs vosApp;

        /// <summary>
        /// Default implementation saves the value to the static VosApp property (if LionFireEnvironment.IsMultiApplicationEnvironment is false).  Set VosApp = null to clear to reset, and change VosAppGetter to avoid this.
        /// </summary>
        public static Func<VosDirs> VosAppGetter => () =>
        {
            var result = new VosDirs(DependencyContext.Current.GetService<IVos>()?.Get());
            if (!LionFireEnvironment.IsMultiApplicationEnvironment)
            {
                vosApp = result;
            }
            return result;
        };

        #endregion

        //public static bool HasActiveData { get { return VosApp != null && VosApp.HasActiveData; } }
        public static IVob ActiveData => VosApp.ActiveData;
        public static IVob Assets => Root["$assets"];

        public static IVob Settings => VosApp.Settings;

        ///// <summary>
        ///// Typically mounted at application install dir, subpath "Data".  Not recommended as a write destination, except for add-on packs.
        ///// REVIEW - Not sure I want to keep it this way.  This could perhaps instead be called BaseData, and Base could be called Core.
        ///// </summary>
        //public static IVob AppData => VosApp.AppData;

        public static IVob Packages => Root[VosPackageLocations.Packages];

        //public static IVob Archives => VosApp.PackageStores;
        //public static IVob Stores => VosApp.Stores;

        private static readonly ILogger l = Log.Get();
    }
}

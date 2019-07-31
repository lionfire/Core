using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Assets
{
    public static class VosAssetsSettings
    {
        #region Settings

        /// <summary>
        /// Set by VosApp.ctor
        /// </summary>
		public static Func<string, Type, string> DefaultPathFromNameForType = DefaultPathFromNameForTypeDefault;

        public static string DefaultPathFromNameForTypeDefault(string name, Type type)
        {
            //return VosApp.GetAssetPath(name, type);
            return LionFire.Assets.AssetPaths.AssetPathFromAssetTypePath(name, type);
        }

        public static Dictionary<Type, Func<string, Type, string>> NameToPathForType = new Dictionary<Type, Func<string, Type, string>>();

        #endregion
    }
}

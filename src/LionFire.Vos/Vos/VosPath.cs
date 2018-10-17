using System.Collections;

namespace LionFire.ObjectBus
{
    public static class VosPath
    {

        #region Conventions

        public const string CachePrefix = "_#"; // FUTURE: Make this a configurable convention
        public const char LayerDelimiter = '|';
        public const char LocationDelimiter = '^';

        #endregion        

        #region VobHandle

        // DUPLICATE See also VosAssets.AssetPathToVobHandle
        // SIMILAR - REFACTOR See also VosContext resolver?

#if AOT
		public static IVobHandle PathToVobHandle(this string path, string package=null, string
		                                         store=null, Type T=null)
#else
        public static VobHandle<T> PathToVobHandle<T>(this string path, string package = null, string
store = null)
where T : class
#endif
        {
#if AOT
			if(T==null)throw new ArgumentNullException("T");
#endif
            Vob vob;

            var context = VosContext.Current;

            if (context != null)
            {
                if (package == null) package = context.Package;
                if (store == null) store = context.Store;
            }

            if (package != null)
            {
                if (store == null)
                {
                    vob = VosApp.Instance.Packages[package][path];
                }
                else
                {
                    vob = VosApp.Instance.PackageStores[package][store][path];
                    //vob = VosApp.Instance.Archives[location][VosPath.PackageNameToStorageSubpath(package)][path];
                }
            }
            else // package == null
            {
                if (store == null)
                {
                    vob = V.Root[path];
                }
                else
                {
                    //l.Trace("Location only, no package? - prob fine but not planned for existing apps");
                    vob = VosApp.Instance.Stores[store][path];
                }
            }
#if AOT
            return vob.ToHandle(T);
#else
            return vob.ToHandle<T>();
#endif
        }

        #endregion

        #region Packages

        public const string PackagePrefix = "[";

        public static string PackageNameToStorageSubpath(string packageName)
        {
            return PackagePrefix + packageName + "]";
        }

        #endregion

        #region Children

        public static bool IsHidden(string childName)
        {
            return childName.StartsWith(PackagePrefix);
        }

        #endregion

        #region MachineSubpath

        public static string MachineSubpath { get { return "/Machine/" + LionEnvironment.MachineName + "/"; } }
        public static string MachineSubpathWithSeparators { get { return "/Machine/" + LionEnvironment.MachineName + "/"; } }

        #endregion

        #region Type Encoding 
        // MOvE this to a separate encoding class?

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        public const char TypeDelimiter = '(';
        public const char TypeEndDelimiter = ')';

        public static string GetTypeNameFromFileName(string fileName)
        {
            int index = fileName.IndexOf(TypeDelimiter);
            if (index == -1) return null;
            return fileName.Substring(index, fileName.IndexOf(TypeEndDelimiter, index) - index);
        }

        //private static string GetDirNameForType(string filePath)
        //{
        //    var chunks = VosPath.ToPathArray(filePath);
        //    if (chunks == null || chunks.Length == 0) yield break;
        //    string parentDirName = chunks[chunks.Length - 1];

        //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
        //}

        #endregion

        public static string GetTypeNameFromPath(string path)
        {
            int index = path.IndexOf(VosPath.TypeDelimiter);
            if (index == -1) return null;
            return path.Substring(index, path.IndexOf(VosPath.TypeEndDelimiter, index) - index);
        }
    }
}

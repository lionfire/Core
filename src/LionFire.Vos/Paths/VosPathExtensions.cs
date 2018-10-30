namespace LionFire.Vos
{
    public static class VosPathExtensions
    {
        #region VobHandle

        // DUPLICATE See also VosAssets.AssetPathToVobHandle
        // SIMILAR - REFACTOR See also VosContext resolver?

#if TOPORT
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

#endif

        #endregion
    }
}

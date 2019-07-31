using LionFire.ObjectBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Vos;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LionFire.Referencing;

namespace LionFire.Assets
{

    /// <summary>
    /// REFACTOR - streamline all the asset reference classes: AssetID, AssetReference, AssetIdentifier, HAsset
    /// </summary>
    public static class AssetReferenceResolver
    {

        #region Individual Assets

        //#if !AOT
        public static VobHandle<AssetType> AssetNameToHandle<AssetType>(this string name, string package = null, string location = null, bool ignoreContext = false, Type concreteType = null
            //, bool downcastIfInterfaceType = true
            )
            where AssetType : class
        {
            string path = LionPath.Combine(AssetPaths.GetAssetTypeFolder(concreteType ?? typeof(AssetType)), name);
            // REVIEW ENH - cast to ConcreteType?

            //#if SanityChecks
            //            if (downcastIfInterfaceType && typeof(AssetType).IsInterface && typeof(AssetType) != concreteType)
            //            {
            //                return AssetPathToHandle
            //                l.Error("AssetReferenceResolver.AssetNameToHandle: typeof(T) (" + typeof(AssetType) + ") != concreteAssetType (" + concreteType + ").  TODO: implement VobHandle downcast. For name: " + name + " Returning: " + result + " " + Environment.StackTrace);
            //            }
            //#endif
            var result = path.AssetPathToHandle<AssetType>(package, location, ignoreContext: ignoreContext);
            return result;
        }
        //#endif
        public static IVobHandle<object> AssetNameToHandle(this string name, string package = null, string location = null, bool ignoreContext = false, Type concreteAssetType = null, Type T = null)
        {
            if (T == null) throw new ArgumentNullException("T");

            string path = LionPath.Combine(AssetPaths.GetAssetTypeFolder(concreteAssetType ?? T), name); // TOTEST
            return path.AssetPathToHandle(package, location, ignoreContext, concreteAssetType ?? T); // TOTEST
        }

        //#if !AOT

        public static VobHandle<T> AssetPathToHandle<T>(this string assetPath, string package = null, string location = null, bool ignoreContext = false, Type concreteType = null)
            where T : class
        {
            //if (concreteType != null) { l.Debug("Not implemented: casting to concrete type."); } // Needs further analysis about how to best handle this - TODO
            //return (VobHandle<T>)AssetPathToHandle(assetPath, package, location, ignoreContext, concreteType ?? typeof(T));
            return (VobHandle<T>)AssetPathToHandle(assetPath, package, location, ignoreContext, typeof(T));
        }

        //#endif

        public static VobHandle<AssetType> ToVobHandle<AssetType>(this AssetIdentifier<AssetType> id, string package = null, string
                                                    location = null, bool ignoreContext = true, Type T = null)
            where AssetType : class
        {
            return (VobHandle<AssetType>)id.AssetPath.AssetPathToHandle(package: id.Package, location: id.Location, ignoreContext: false, type: typeof(AssetType));
        }

        public static IVobHandle<object> AssetPathToHandle(this string assetPath, string package = null, string
                                                        location = null, bool ignoreContext = true, Type type = null)
        {
            throw new NotImplementedException();
#if TOPORT
            if (type == null) throw new ArgumentNullException("T");

            Vob vob;

            if (ignoreContext)
            {
                vob = V.Assets[assetPath];
            }
            else
            {
                //var context = VosContext.Current;

                //if (context != null)
                //{
                //    // Unused
                //    //if (package == null) package = context.Package;
                //    //if (location == null) location = context.Store;
                //}
                //else
                //{
                //    context = new VosContext();
                //}
                //if (package != null) context.Package = package;
                //if (location != null) context.Store = location;
                //#error Clobbers current context!

                //vob = context.Root;

                vob = VosContext.DefaultResolver.GetVobRoot(package: package, location: location);

                vob = vob[VosApp.AssetsSubpathChunks][assetPath];
            }
            //l.Trace("AssetPathToHandle: " + vob.Path);

            //if (package != null)
            //{
            //    if (location == null)
            //    {
            //        vob = VosApp.Instance.Packages[package][VosApp.AssetsSubpathChunks][assetPath];
            //    }
            //    else
            //    {
            //        vob = VosApp.Instance.PackageStores[VosPath.PackageNameToStorageSubpath(package)][location][VosApp.AssetsSubpathChunks][assetPath];
            //    }
            //}
            //else // package == null
            //{
            //    if (location == null)
            //    {
            //        vob = V.Assets[assetPath];
            //    }
            //    else
            //    {
            //        l.Warn("UNTESTED / UNEXPECTED: Location only, no package? - prob fine but not planned for existing apps");
            //        vob = VosApp.Instance.Stores[location][VosApp.AssetsSubpathChunks][assetPath];
            //    }
            //}
            if (type != null) throw new NotImplementedException("Not implemented yet when type is specified");
            return vob.GetHandle<object>();
            //            return vob.ToHandle<T>();
#endif
        }

#endregion

        private static ILogger l = Log.Get();

    }
}

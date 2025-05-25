#if TOPORT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LionFire.Serialization;
using LionFire.Vos;

namespace LionFire.Assets
{
    /// <summary>
    /// New IAsset? Simplified?  REVIEW - how to best deal with Assets now that VOS is up and working
    /// REVIEW - Need to have AssetType, to support multiple concrete types for a common base type?
    /// </summary>
	//public interface IAssetBase 
        //: IReferenceable 
        // Was IHRAsset
    //{
        //string AssetSubPath { get; }
    //}

#if !AOT

    public interface IAsset<AssetType>
        :
        //IAssetBase,
        IHasHAsset<AssetType>
        where AssetType : class
    {

    }
#endif

    public interface IAsset : IHasReadHandle
    {
        Type Type
        {
            get;
        }
        string AssetTypePath { get; }

        //// Used to get default save/load directory.  DEPRECATED: In OBus, put an attribute on a base class
        //// (or interface?) and search for a default type name that way.
        //Type Type { get; }
        AssetID ID { get; }
    }

    //public static class IAssetExtensions
    //{
    //    #region Save

    //    //public static VobHandle<T> GetSaveVobHandle<T>(this T asset)
    //    //    where T : class, IAsset
    //    //{
    //    //    var packageName = asset.ID.PackageName ;
    //    //    if(packageName != null)
    //    //    {

    //    //    }

    //    //    var savePath = GetSavePath(asset);

    //    //    new VobHandle<T>(savePath)
    //    //    {
    //    //        Lay
    //    //    }
    //    //}

    //    //public static string GetSavePath(this IAsset asset) OLD
    //    //{
    //    //    if (asset == null) throw new ArgumentNullException("asset");
    //    //    if (asset.ID == null) throw new ArgumentNullException("asset.ID");

    //    //    Package savePackage = asset.ID.Package ?? AssetContext.Current.DefaultSavePackage;
    //    //    if (savePackage == null)
    //    //    {
    //    //        throw new Exception("Cannot determing save location because package is not specified and AssetContext.Current.DefaultSavePackage is not set.");
    //    //    }
    //    //    return Path.Combine(savePackage.PackageDirectory, asset.ID.Path);
    //    //}

    //    //public static void Save<T>(this T asset, Package package) UNUSED
    //    //    where T : class, IAsset, new() // TODO: Try removing new()
    //    //{
    //    //    asset.ID.Package = package;
    //    //    asset.Save();
    //    //}

    //    //public static void Save(this IAsset asset, bool incrementVersion = true)
    //    //{
    //    //    SerializationFacility.Serialize(asset.GetSavePath(), asset);
    //    //}

    //    private static ILogger l = Log.Get();

    //    //public static T Save<T>(this T asset, bool incrementVersion = true)
    //    //    //where T : class, IAsset, new()
    //    //    where T : class, IAsset
    //    //{
    //    //    //var vh = GetSaveVobHandle<T>(asset);
    //    //    //vh.Object = asset;
    //    //    //vh.Save();


    //    //    if (asset == null) throw new ArgumentNullException("asset");
    //    //    if (asset.ID == null)
    //    //    {
    //    //        IHasHandle h = asset as IHasHandle;
    //    //        if (h != null)
    //    //        {
    //    //            //l.Trace("SAViNG: asset as IHasHandle");
    //    //            if (h.Handle == null)
    //    //            {
    //    //                throw new ArgumentNullException("asset.ID == null &&  ((IHasHandle)asset).Handle == null");
    //    //            }
    //    //            if (h.Handle.HasObject && !object.ReferenceEquals(h.Handle.Object, asset))
    //    //            {
    //    //                l.Warn("Save: !object.ReferenceEquals(h.Handle.Object, asset)");
    //    //                h.Handle.Object = asset;
    //    //            }
    //    //            h.Handle.Save();
    //    //            return asset;
    //    //        }
    //    //        else
    //    //        {
    //    //            throw new ArgumentNullException("asset.ID");
    //    //        }
    //    //    }
    //    //    //var context = VosContext.Current;

    //    //    //Vob root = context.Root;
    //    //    ////string packageName = AssetContext.Current.DefaultSavePackage.Name;

    //    //    //Vob vob = V.Packages[packageName][asset.ID.Path];
    //    //    //Vob vob = root[asset.ID.Path];
    //    //    //var ha = new HAsset<T>(asset, asset.ID.Name);


    //    //    var vh = VosAssets.AssetNameToHandle<T>(asset.ID.Name);
    //    //    //var vh = asset.ID.ToVosHandle<T>();
    //    //    vh.Object = asset;
    //    //    vh.Save();
    //    //    //vob.Save(asset);

    //    //    //VobHandle<T> vh = new VobHandle<T>(packageRoot, asset.ID.Path);
    //    //    //vh.Package = asset.ID.PackageName;
    //    //    //vh.Object = asset;
    //    //    //vh.Save();

    //    //    //Vob vob = VosApp.Instance.ActiveData[asset.ID.Path];
    //    //    //vob.Object = asset;
    //    //    //vob.Save(asset.ID.PackageName);

    //    //    //SerializationFacility.Serialize(asset.GetSavePath(), asset); OLD
    //    //    return asset;
    //    //}

    //    //public static void SaveUser() // FUTURE: Instead of saving to a user overlay, create user-owned packages that are themselves overlays on other packages.
    //    //{
    //    //}

    //    #endregion

    //}

}
#endif
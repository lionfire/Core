using LionFire.Assets;
using LionFire.Vos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public static class AssetExtensions
    {
        public static VHAsset<T> GetAsset<T>(this string assetSubpath)
            where T : class
        {
            return (VHAsset<T>)assetSubpath;
        }
    }
}

namespace LionFire.Vos
{
    
    /// <summary>
    /// Asset is a convenience VHReference that is used to get assets by name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VHAsset<T> : VHReference<T>
        where T : class
    {
        public static string AssetSubpathToPath(string assetSubpath)
        {
            string result = AssetPaths.AssetPathFromAssetTypePath(assetSubpath, typeof(T)); ;
            
            return LionPath.Combine(V.Assets.Path, result);
        }
        public static string[] AssetSubpathToPath(string[] assetSubpathComponents)
        {
            //string result = assetSubpath;
            //return result;
            throw new NotImplementedException();
        }

        public static string PathToAssetSubpath(string path)
        {
            string result = path;
            return result;
        }
        public static string[] PathToAssetSubpath(string[] pathComponents)
        {
            //string result = assetSubpath;
            //return result;
            throw new NotImplementedException();
        }

        #region Construction

        public VHAsset() { }
        public VHAsset(string assetSubpath) : base(AssetSubpathToPath(assetSubpath)) { }
        public VHAsset(params string[] assetSubpathComponents) : base(AssetSubpathToPath(assetSubpathComponents)) { }

        public VHAsset(VobHandle<T> vh) : base(vh) { }

        public static implicit operator T(VHAsset<T> me)
        {
            return me.Handle.Object;
        }
        public static implicit operator VHAsset<T>(string vosPath)
        {
            return new VHAsset<T>(vosPath);
        }
        public static implicit operator VHAsset<T>(string[] vosPath)
        {
            return new VHAsset<T>(vosPath);
        }

        #region To/From VobHandle<>

        public static implicit operator VHAsset<T>(VobHandle<T> vh)
        {
            return new VHAsset<T>(vh);
        }

        public static implicit operator VobHandle<T>(VHAsset<T> vh)
        {
            return vh.Handle;
        }

        #endregion

        #endregion

    }

}

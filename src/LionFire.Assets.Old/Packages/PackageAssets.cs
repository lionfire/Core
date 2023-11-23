using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LionFire.Assets
{


    //public class PackageAssets
    //{
    //    public readonly Package Package;
    //    public PackageAssets(Package package) { this.Package = package; }
    //}

    public class PackageAssets
    {
        public Package Package { get; set; }

        public Dictionary<Type, object> AssetsByType = new Dictionary<Type, object>();
        public object assetsByTypeLock = new object();

        public IEnumerable<AssetReference<AssetType>> GetAssets<AssetType>()
            where AssetType : class, IAsset
        {
            lock (assetsByTypeLock)
            {
                object assetsOfType;
                //List<IAssetReference> assetsOfType;
                if (!AssetsByType.TryGetValue(typeof(AssetType), out assetsOfType))
                {
                    assetsOfType = FindAssetsInDefaultLocation<AssetType>(verifyAssetType: true);
                    AssetsByType.Add(typeof(AssetType), assetsOfType);
                }
                return (IEnumerable<AssetReference<AssetType>>)assetsOfType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="AssetType"></typeparam>
        /// <param name="loadAssets">Loads assets (not recommended for large assets)</param>
        /// <param name="verifyAssetType">Doublecheck that the files in the default location are actually of the expected type.  (If loadFullAssetIDs is true or loadAssets is true, this will be true.)</param>
        /// <param name="loadFullAssetIDs"></param>
        /// <returns>AssetReferences of all assets for the package in the default location for the asset type.</returns>
        public async Task<IEnumerable<AssetReference<AssetType>>> FindAssetsInDefaultLocation<AssetType>(bool loadAssets = false, bool loadFullAssetIDs = false, bool verifyAssetType = false)
            where AssetType : class, IAsset
        {

            string defaultDirectory = AssetPaths.GetAssetTypeFolder(typeof(AssetType));

            string defaultPath = Path.Combine(Package.PackageDirectory, defaultDirectory);

            var assets = new List<AssetReference<AssetType>>();

            defaultPath = defaultPath.TrimEnd('/', '\\');
            l.Info("Package '" + this.Package.AssetTypePath + "' loading assets of type " + typeof(AssetType).Name + " from " + defaultPath);

            if (Directory.Exists(defaultPath))
            {
                await FindAssetsInDirectory<AssetType>(loadAssets, loadFullAssetIDs, verifyAssetType, defaultPath, assets);
            }
            return assets;
        }

        private async Task FindAssetsInDirectory<AssetType>(bool loadAssets, bool loadFullAssetIDs, bool verifyAssetType, string path, List<AssetReference<AssetType>> assets) where AssetType : class, IAsset
        {
            foreach (string filepath in Directory.GetFiles(path))
            {
                if (loadAssets || verifyAssetType || loadFullAssetIDs)
                {
                    l.Trace("Loading asset at " + filepath);
                    AssetType asset;
                    asset = await (new FileReference(filepath)).GetObject<AssetType>();

#if OLD
                    //object deserializedObject = SerializationFacility.Default.Deserialize(filepath);
                    //asset = deserializedObject as AssetType;
                    //if (asset == null)
                    //{
                    //    l.Trace("Ignoring asset of type '" +
                    //        (deserializedObject == null ? "null" : deserializedObject.GetType().Name) +
                    //        "' in search for assets of type '" + typeof(AssetType) + "'.");
                    //    continue;
                    //}
#endif
                    l.Trace("Ignoring asset '" + Path.GetFileName(filepath) + "' in search for assets of type '" + typeof(AssetType) + "'.");

                    if (loadAssets)
                    {
                        assets.Add(new AssetReference<AssetType>(asset));
                        l.Debug("Loaded asset at " + filepath + ": " + asset.ID);
                    }
                    else
                    {
                        // Forget about the asset we just loaded and only use its ID
                        assets.Add(new AssetReference<AssetType>(asset.ID));
                        l.Debug("Loaded asset id at " + filepath + ": " + asset.ID);
                    }
                }
                else
                {
                    //string lastIndexOfString = !String.IsNullOrEmpty(defaultDirectory) ? defaultDirectory : Package.PackageDirectory;
                    string lastIndexOfString = Package.PackageDirectory;

                    string assetPath = filepath.Substring(filepath.LastIndexOf(lastIndexOfString) + lastIndexOfString.Length);
                    assetPath = assetPath.TrimStart('/', '\\');
                    assetPath = assetPath.Substring(0, assetPath.LastIndexOf('.'));
                    assets.Add(new AssetReference<AssetType>(assetPath));

                    l.Debug("Located asset at " + filepath + ": " + assetPath);
                }
            }

            foreach (string directoryPath in Directory.GetDirectories(path))
            {
                FindAssetsInDirectory<AssetType>(loadAssets, loadFullAssetIDs, verifyAssetType, directoryPath, assets);
            }
        }

        private static ILogger l = Log.Get();

    }
}

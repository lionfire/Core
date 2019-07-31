using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using System.IO;
using LionFire.Serialization;
using System.Diagnostics.Contracts;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace LionFire.Assets
{
    public static class AssetManager<AssetType>
        where AssetType : class, IAsset
    {
        //private static LionSerializer serializer = LionSerializers.Json; OLD

        public static MultiBindableCollection<AssetType> instancesLoaded = new MultiBindableCollection<AssetType>();
        private static Dictionary<string, AssetType> instancesLoadedByPath = new Dictionary<string, AssetType>();
        private static object instancesLock = new object();

        public static MultiBindableCollection<AssetType> InstancesLoaded // TODO - readonly
        {
            get { return instancesLoaded; }
        }

        private static void Register(AssetType item)
        {
            lock (instancesLock)
            {
                var key = item.ID.Key;
                if (!instancesLoadedByPath.ContainsKey(key))
                {
                    instancesLoaded.Add(item);
                    instancesLoadedByPath.Add(key, item);
                }
            }
        }

        public static AssetContext EffectiveContext => AssetContext.EffectiveContext;


        ///// <summary>
        ///// would have to go thru the AssetContext
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public static bool CanLoad(string path)
        //{
        //    // TODO: Add ruleerror if MapName is not set or is not valid,
        //    return instancesLoadedByPath.ContainsKey(path);
        //}

        private static AssetType TryLoad(AssetID assetID, string assetSavePath)
        {
            throw new NotImplementedException("TODO - use FsOBase?");
#if TOPORT
            AssetType asset;

            lock (instancesLock)
            {
                if (instancesLoadedByPath.TryGetValue(assetID.Key, out asset))
                {
                    return asset;
                }
            }

            if (!FileSerializationFacility.Exists(assetSavePath))
            {
                //throw new Exception("Asset not found in specified package at path: " + assetSavePath);
                l.Debug("Asset not found in specified package at path: " + assetSavePath);
                return null;
            }
            try
            {
#if AOT
				asset = (AssetType)SerializationFacility.Deserialize(assetSavePath, typeof(AssetType));
#else
                asset = FileSerializationFacility.Deserialize<AssetType>(assetSavePath);
#endif
            }
            catch
            {
                l.Error("Failed to deserialize asset at path: " + assetSavePath);
                throw;
            }

            ValidateAssetID(assetID, asset);
            Register(asset);
            return asset;
#endif
        }

        // REFACTOR?
        // MOVE to AssetOBase?
        public static AssetType Load(AssetID assetID, AssetContext assetContext = null)
        {
            if (assetContext == null) assetContext = EffectiveContext;

            if (assetContext == null) throw new Exception("No asset context.  Be sure to set AssetContext.Global to include your default search paths.");

            lock (assetContext.instancesLoadedByPathLock)
            {
                object loadedAssetObj;

                if (assetContext.instancesLoadedByPath.TryGetValue(assetID.Key, out loadedAssetObj))
                {
                    AssetType loadedAsset = loadedAssetObj as AssetType;
                    if (loadedAsset != null)
                    {
                        return loadedAsset;
                    }
                    else
                    {
                        l.Warn("Unexpected type for loaded asset: " + loadedAssetObj.GetType().Name);
                    }
                }

                if (!String.IsNullOrEmpty(assetID.PackageName))
                {
                    return LoadFromPackage(assetID, assetContext, assetID.PackageName);
                }
                else
                {
                    foreach (string packageDirectory in
#if AOT
					         (IEnumerable)
#endif
					         assetContext.PackageDirectories)
                    {
                        if (packageDirectory == null)
                        {
                            Contract.Requires(packageDirectory != null, "Null packageDirectory in AssetContext");
                            continue;
                        }
                        AssetType asset = TryLoad(assetID, Path.Combine(packageDirectory, assetID.Path));
                        if (asset != null)
                        {
                            assetContext.instancesLoadedByPath.Add(assetID.Key, asset);
                            return asset;
                        }
                    }
                    throw new Exception("Asset not found in any package: " + assetID.Path);
                }
            }
        }

        // REFACTOR
        public static AssetType LoadFromPackage(AssetID assetID, AssetContext assetContext, string packageName)
        {
            if (assetContext == null) assetContext = EffectiveContext;
            string packagePath = assetContext.GetPathForPackage(packageName);
            if (packagePath == null)
            {
                throw new Exception("Package not found in current context: " + assetID.PackageName);
            }

            string assetPath = Path.Combine(packagePath, assetID.Path);

            AssetType asset = TryLoad(assetID, assetPath);
            if (asset == null) throw new AssetNotFoundException("Asset not found in specified package at path: " + assetPath);

            return asset;
        }

        private static void ValidateAssetID(AssetID assetID, AssetType asset, bool versionMatch = false)
        {
            if (asset == null) throw new Exception("Failed to deserialize asset: " + assetID);
            if (assetID.Path != asset.ID.Path) throw new Exception("Loaded asset path mismatch");
            // TODO REVIEW - AssetID checks
            // TODO REVIEW - checksum / key checks
        }

        public static AssetType Load(IHRAsset<AssetType> assetReference)
        {
            throw new NotImplementedException("TODO: AssetReference shouldn't hold the asset.  Use HAsset instead.");
            //assetReference.Asset = Load(assetReference.ID);
            //return assetReference.Asset;
        }

        //public static IEnumerable<AssetReference<AssetType>> Instances
        //{
        //    get
        //    {                
        //        if (instances == null)
        //        {
        //            LoadInstances();
        //        }
        //        return instances;
        //    }
        //}private static IEnumerable<AssetReference<AssetType>> instances;

        //public static IEnumerable<AssetID> InstanceIDs
        //{
        //    get
        //    {
        //        if (instanceIDs == null)
        //        {
        //            LoadInstanceIDs();
        //        }
        //        return instanceIDs;
        //    }
        //}
        //private static IEnumerable<AssetID> instanceIDs;

        //public static void LoadInstanceIDs(string directory = null, AssetContext context = null, List<AssetID> ids = null)
        //{

        //    if(directory == null)
        //    {
        //        context = AssetContext.Current;
        //        ids = new List<AssetID>();
        //    foreach(string packageDir in AssetContext.Current.PackageDirectories)
        //    {
        //        LoadInstanceIDs(packageDir,context, ids);
        //    }

        //        instances = null;
        //    //instanceIDs = ;

        //    }
        //    else
        //    {
        //        foreach (string filePath in Directory.GetFiles())
        //        {
        //            LoadInstanceIDs(packageDir, context, ids);
        //        }

        //        foreach (string packageDir in AssetContext.Current.PackageDirectories)
        //        {
        //            LoadInstanceIDs(packageDir, context, ids);
        //        }

        //    }

        //}


        //public static T Load(string path, string packageName = null)
        //{
        //    if (String.IsNullOrEmpty(path)) return null;

        //    if (instancesLoadedByPath.ContainsKey(path)) return instancesLoadedByPath[path];

        //    //if (typeof(T) == typeof(Package)) throw new ValorException("Cannot load package with a path containing a package specifier");

        //    if (path.Contains(";"))
        //    {
        //        if (packageName != null) throw new ArgumentException("Illegal ; in path.  (PackageName already specified.)");

        //        var pathSplit = path.Split(';');
        //        return Load(pathSplit[1], pathSplit[0]);

        //        Package package = PackageManager.Instance.GetPackage(packageName);

        //        string packageDirectory = package.PackageDirectory;


        //        Path.Combine(packageDirectory, "");
        //    }
        //    else
        //    {
        //        //PackageManager.Instance.SearchPaths
        //    }
        //}

        //public static void Save(T asset)
        //{
        //    if(asset.Package == null) throw new ArgumentException("Package not set.");

        //    var id = asset.ID;

        //    string assetFileName = Path.Combine(asset.Package.PackageDirectory, asset.ID.Path);
        //    assetFileName += serializer.FileDotExtension;
        //}

        private static ILogger l = Log.Get();

    }

    //public class AssetLoader
    //{
    //    public 
    //}

    //public static class AssetExtensions
    //{
    //    public static void Save<T>(this T asset)
    //        where T : class, IAsset
    //    {
    //        AssetManager<T>.Save(asset);
    //    }

    //    //public static T Load<T>(string assetPath)
    //    //{

    //    //}
    //}

    //public class AssetManager
    //{
    //    #region (Private) Fields

    //    private static ILogger l = Log.Get();
    //    private readonly ValorApp app;

    //    #endregion

    //    public static AssetManager Current { get { return current; } }
    //    private static AssetManager current;

    //    public string[] AssetPaths = new string[] { "data" };

    //    #region Construction

    //    public AssetManager(ValorApp app)
    //    {
    //        if (current == null) current = this;
    //        this.app = app;
    //    }


    //    #endregion

    //    public IEnumerable<AssetDescriptor> LocalAssets
    //    {
    //        get
    //        {
    //            return new AssetDescriptor[] 
    //            {
    //                new AssetDescriptor{ Name="Map1", Type="Map" },
    //                new AssetDescriptor{ Name="Map2", Type="Map" },
    //            }; // STUB

    //            //foreach (string dataPath in AssetPaths)
    //            //{
    //            //    //foreach(
    //            //}
    //        }
    //    }

    //    public IEnumerable<IAsset> LoadedAssets { get { return loadedAssets.Values; } }

    //    Dictionary<string, IAsset> loadedAssets = new Dictionary<string, IAsset>();

    //    public void Register<T>(T asset) where T : IAsset
    //    {
    //        if (asset == null) throw new ArgumentNullException();
    //        if (loadedAssets.ContainsKey(asset.Id.Name))
    //        {
    //            throw new DuplicateNotAllowedException("Asset already registered: " + asset.Id.Name);
    //        }
    //        l.Info("Loaded " + asset.Id.Type + " " + asset.Id.Name);
    //        loadedAssets.Add(asset.Id.Name, asset);
    //    }

    //    #region Unload

    //    public void Unload<T>(T asset) where T : IAsset
    //    {
    //        if (asset == null) throw new ArgumentNullException();
    //        loadedAssets.Remove(asset.Id.Name);
    //    }
    //    public void Unload(string assetName)
    //    {
    //        if (String.IsNullOrEmpty(assetName)) throw new ArgumentNullException();
    //        loadedAssets.Remove(assetName);
    //    }

    //    public void UnloadAll()
    //    {
    //        l.Info("Unloading all assets.");
    //        loadedAssets.Clear();
    //    }

    //    //public void UnloadUnused() // FUTURE OPTIMIZE

    //    #endregion

    //    public static Dictionary<string, Map> loadedMaps = new Dictionary<string, Map>();

    //    public T Load<T>(string assetName)
    //        //where T : IAsset
    //    {
    //        switch (typeof(T).Name)
    //        {
    //            case "Map":
    //                // STUB
    //                {
    //                    if (loadedMaps.ContainsKey(assetName)) return (T)(object) loadedMaps[assetName];

    //                    Map map = new Map();

    //                    map.Prototypes.Add(new ValorEntity() { MeshName = "box.mesh", Name = "box" });
    //                    map.Prototypes.Add("plane", new ValorEntity() { MeshName = "plane.mesh", Name = "plane" });
    //                    map.Prototypes.Add("OgreHead", new ValorEntity() { MeshName = "ogrehead.mesh", Name = "OgreHead" });

    //                    if (!loadedMaps.ContainsKey(assetName))
    //                    {
    //                        loadedMaps.Add(assetName, map);
    //                    }
    //                    else
    //                    {
    //                        loadedMaps[assetName] = map;
    //                    }
    //                    return (T) (object)map;
    //                }
    //            case "TheatreEngine":
    //                if (loadedAssets.ContainsKey(assetName))
    //                {
    //                    TheatreEngine engine = loadedAssets[assetName] as TheatreEngine;
    //                    if (engine == null)
    //                    {
    //                        throw new ValorException(assetName + " is not a TheatreEngine"); // TODO - better exception type
    //                    }
    //                    else
    //                    {
    //                        return (T)(object)engine;
    //                    }
    //                }
    //                else
    //                {
    //                    throw new ValorException("TheatreEngine '" + assetName + "' was not found."); // TODO - better exception type
    //                }
    //            case "RuleSet":
    //                var rules = new RuleSet();
    //                rules.Id = new AssetDescriptor
    //                {
    //                    Name = "DefaultRules",
    //                    DisplayName = "Default",
    //                };
    //                return (T) (object)rules;
    //                break;
    //            default:
    //                throw new NotSupportedException();
    //        }
    //    }

    //    public string[] GetLocalNames<T>() where T : IAsset
    //    {
    //        return new string[] { "Test1", "Test2", "Test3" };
    //        //switch (typeof(T).Name)
    //        //{
    //        //    case "Map":
    //        //        // STUB
    //        //        {

    //        //        }
    //        //        break;
    //        //}
    //        //throw new NotSupportedException();
    //    }

    //    //public static class MapLoader
    //    //{
    //    //    static MapLoader()
    //    //    {
    //    //        // TEMP TEST STUB
    //    //        Load("TestMap1");
    //    //        Load("TestMap2");
    //    //        Load("TestMap3");
    //    //    }

    //    //    public static Map Load(string mapName)
    //    //    {
    //    //        return ValorApp.Current.AssetManager.Load<Map>(mapName);
    //    //    }

    //    //    public static string[] AvailableMaps
    //    //    {
    //    //        get
    //    //        {
    //    //            // STUB
    //    //            return loadedMaps.Keys.ToArray();
    //    //        }
    //    //    }
    //    //}
    //}
}

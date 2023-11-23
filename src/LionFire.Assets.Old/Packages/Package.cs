using System;
using System.Text;
using System.Reflection;
//using System.Xml.Serialization;
using LionFire.Serialization;
using LionFire.Collections;
using LionFire.ObjectBus;
using Microsoft.Extensions.Logging;
using LionFire.IO.Filesystem;
using LionFire.ObjectBus.Filesystem;

namespace LionFire.Assets
{
    //enum SavePolicy
    //{
    //    Unspecified,
    //    SaveIfMissing,
    //}

    //enum SaveLocation
    //{
    //    Unspecified,
    //}

    /// <summary>
    /// TODO Features: Overlayable, stackable, mods, baselines, etc. in a tree.
    /// TODO: Make Mod a generic stackable data hierarchy
    /// </summary>
    public class Package :
        AssetBase<Package>,
        //MultiTypeVobo<Package>, 
        INotifyDeserialized
    //, ISerializableAsset
    {
        // This could allow user to specify save as Zip archive vs text files, etc.
        //public SerializationParameters SerializationParameters = new SerializationParameters();

        /// <summary>
        /// The on-disk directory location of the package
        /// </summary>
        [Ignore]
        public string PackageDirectory { get; set; } // TODO: Get from AssetID?

        public override string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        } private string displayName;

#region Assets

        public PackageAssets Assets
        {
            get
            {
                if (assets == null)
                {
                    assets = new PackageAssets { Package = this };
                }
                return assets;
            }
        } private PackageAssets assets;

#endregion

        //Package IAsset.Package { get { return this; } set { if (value == this) return; throw new NotSupportedException("Cannot set Package on a Package."); } }

        //    #region Assets

        //    #region Loaded

        //    public MultiBindableDictionary<Type, MultiBindableCollection<IAsset>> instancesLoaded = new MultiBindableDictionary<Type, MultiBindableCollection<IAsset>>()
        //    {
        //        AutoCreate = (t) => new MultiBindableCollection<IAsset>(),
        //    };
        //    private MultiBindableDictionary<Type, MultiBindableDictionary<string, IAsset>> instancesLoadedByPath = new MultiBindableDictionary<Type, MultiBindableDictionary<string, IAsset>>()
        //    {
        //        AutoCreate = (t) => new MultiBindableDictionary<string, IAsset>(),
        //    };

        //    public MultiBindableCollection<IAsset> GetInstancesLoaded<InstanceType>()
        //    {
        //        return instancesLoaded[typeof(InstanceType)];
        //    }

        //    #endregion

        //    private string GetItemSavePath(IAsset asset)
        //    {
        //        string savePath = System.IO.Path.Combine(PackageDirectory, asset.ID.Path);
        //        return savePath;
        //    }

        //    public void Add<AssetType>(AssetType item)
        //        where AssetType : class, IAsset
        //    {
        //        instancesLoaded[typeof(AssetType)].Add(item);
        //        instancesLoadedByPath[typeof(AssetType)].Add(item.ID.Name, item);

        //        SaveIfMissing<AssetType>(item);
        //    }

        //    public void SaveIfMissing<AssetType>(AssetType asset)
        //        where AssetType : class, IAsset
        //    {
        //        string savePath = GetItemSavePath(asset);
        //        if (!SerializationFacility.Exists<AssetType>(savePath))
        //        {
        //            Save<AssetType>(asset, savePath);
        //        }
        //    }

        //    public void Save<AssetType>(AssetType item, string savePath = null)
        //        where AssetType : class, IAsset
        //    {
        //        SerializationFacility.Serialize(savePath, item, SerializationParameters);
        //    }

        //    public AssetType LoadAsset<AssetType>(string path)
        //where AssetType : IAsset
        //    {
        //        string assetPath = System.IO.Path.Combine(PackageDirectory, path);

        //        AssetManager<AssetType>.Load(
        //        return SerializationFacility.Deserialize<AssetType>(assetPath);
        //    }           

        //    #endregion

#region OLD - TODO - redo this

        //private ModuleTypeManager moduleTypes = new ModuleTypeManager();
        //private UnitSpecificationManager unitSpecifications = new UnitSpecificationManager();

        //public UnitSpecificationManager UnitSpecifications // TEMP
        //{
        //    get { return unitSpecifications; }
        //    set { unitSpecifications = value; }
        //}

        //public bool EnabledByDefault { get; set; }

        //public string[] Dependencies { get; set; }

        //private AIManager ais; // REVIEW was this needed for something?

        //public AIManager AIs
        //{
        //    get { return ais; }
        //    set { ais = value; }
        //}

#endregion

        public Package()
        {
            //Assets = new PackageAssets(this);
        }
        public Package(string assetPath)
            : base(assetPath)
        {
            //this.ID = new AssetID(name);
            //this.ID.Package = this;
        }

        //public Package(AssetID id)
        //{
        //    // OLD
        //    this.ID = id;
        //    this.ID.Package = this;
        //    this.PackageDirectory = Path.Combine(AssetContext.Global.DefaultPackageDirectory, id.Name);
        //}

        //public Package(string name, string version) : this()
        //{
        //    this.name = name;
        //    this.version = version;

        //    //this.ais = new AIManager();

        //    Load();
        //}

        //public void Load() // OLD
        //{
        //    this.moduleTypes.Load(this);
        //    this.unitSpecifications.Load(this);
        //}

        //public void Save()
        //{
        //    this.moduleTypes.Save();
        //    this.unitSpecifications.Save();
        //}

        //public void Save(UnitSpecification unitVariant)
        //{
        //    unitVariant.Save(this.Path);
        //}
        //public void Save(IModuleType moduleType)
        //{
        //    moduleType.Save(this.Path);
        //}

        //public void Delete(UnitSpecification unitVariant)
        //{
        //    unitVariant.Delete(this.Path);
        //}
        //public void Delete(IModuleType moduleType)
        //{
        //    moduleType.Delete(this.Path);
        //}

        //public string ModPartialPath(string modName, string version)
        //{             
        //    return modName + System.IO.Path.DirectorySeparatorChar + version;
        //}

        //public ModuleTypeManager ModuleTypeManager
        //{
        //    get { return moduleTypes; }
        //}

        //public string Path
        //{
        //    get
        //    {
        //            return PackageManager.Instance.ModVersionPath(name, version);
        //    }
        //}

        //public string PartialPath
        //{
        //    get
        //    {
        //        return PackageManager.Instance.ModPartialPath(name, version);
        //    }
        //}

        public bool EnabledByDefault { get; set; }

        public void OnDeserialized()
        {
            throw new NotImplementedException();
            //this.ID.Package = this; // TODO
        }

    }
}

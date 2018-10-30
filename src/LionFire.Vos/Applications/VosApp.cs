//#define ASSETS_SUBPATH // Prefer this off?  TODO - make sure this works for packages

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Assets;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{
    public class VosApp
    {
        #region (Static) Instance Accessor

        public static bool HasInstance { get { return instance != null; } }
        public static VosApp Instance
        {
            get { return instance; }
            set
            {
                lock (instanceLock)
                {
                    if (instance != null) throw new AlreadyException("VosApp.Instance has already been set.");
                    instance = value;
                }
            }
        }
        private static VosApp instance;
        protected static object instanceLock = new object();

        public static T GetInstance<T>()
            where T : VosApp, new()
        {
            if (!VosApp.HasInstance)
            {
                lock (VosApp.instanceLock)
                {
                    if (!VosApp.HasInstance)
                    {
                        new T();
                    }
                }
            }
            return (T)VosApp.Instance;
        }

        #endregion

        #region Configuration

        public const bool NoFS =
#if AOT
			true;
#else
            false;
#endif

        #endregion

        public PackageMounter PackageMounter { get; protected set; }

        #region Construction

        /// <summary>
        /// Default Init.  TODO: Register VosApp type in the Type Resolver system, and fall back to VosApp if none specified.
        /// </summary>
        public static VosApp InitVosApp()
        {
            if (VosApp.Instance == null)
            {
                var vosApp = new VosApp();
                l.Trace("Initialized " + vosApp.GetType().Name);
            }
            return VosApp.Instance;
        }

        protected VosApp()
        {
            VosApp.Instance = this;

            this.PackageMounter = new PackageMounter();

            OnConstructing();

            Initialize();

            VosAssetsSettings.DefaultPathFromNameForType = VosApp.NameToPathForType;
        }

        protected virtual void OnConstructing()
        {
        }

        private void Initialize()
        {
            MountDefaultMounts();

            var ev = MountingAppMounts;
            if (ev != null) ev();

            Initialize_Packages();

#if RKV
            Vob vCounter = VosApp.Instance.DBs["RKV"]["Counter"];
            if (vCounter.TryEnsureRetrieved())
            {
                l.LogCritical("Got counter: " + vCounter.Object);
                int x = (int)vCounter.Object;
                x++;
                vCounter.Object = x;
                vCounter.Save();
            }
            else
            {
                l.LogCritical("Initializing counter");
                int number = 1;
                vCounter.Object = number;
                vCounter.Save();
            }
#endif
        }

        private void Initialize_Packages()
        {
            if (LionFire.Applications.LionFireApp.Current != null && LionFire.Applications.LionFireApp.Current.IsPackagesEnabled)
            {
                InitPackageDirectories();

                this.PackageMounter.MountAllPackages();

                //this.PackageMounter.InitVos();
            }
        }


        public event Action MountingAppMounts;


        private void MountDefaultMounts()
        {
            foreach (var mount in VosApp.Instance.GetDefaultMounts())
            {
                //vosMounts.Add(mount);
                if (mount.MountOptions.MountAtStartup)
                {
                    mount.IsEnabled = true;
                }
            }
        }


        
        #endregion

        #region Packages

        public virtual void InitPackageDirectories()
        {
            bool useVar = VosDiskPaths.AppBase != VosDiskPaths.VarBase;

            List<string> packagePaths = new List<string>();

            var packageDirectories = PackageMounter.PackageDirectories;

            #region BasePacks

            packagePaths.Add(VosDiskPaths.AppBase);
            {
                PackageDirectory pd = new PackageDirectory()
                {
                    Path = VosDiskPaths.AppBase,
                    LocationName = VosStoreNames.AppBase,
                    MountOptions = new MountOptions()
                    {
                        IsReadOnly = true, // Base = readonly
                        ReadPriority = -1000,
                        WritePriority = 11000, // Shouldn't have permissions
                    },
                };
                packageDirectories.Add(pd);
            }

            if (useVar)
            {
                packagePaths.Add(VosDiskPaths.VarBase);
                {
                    PackageDirectory pd = new PackageDirectory()
                    {
                        Path = VosDiskPaths.VarBase,
                        LocationName = VosStoreNames.VarBase,
                        MountOptions = new MountOptions()
                        {
                            IsReadOnly = true, // Base = readonly
                            ReadPriority = -100,
                            // MaxValue for WritePriority means can only be written to if layer name is manually specified
                            WritePriority = 10500, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

#if ENABLE_APPDATA_FALLBACK
            packageDirectories.Add(new PackageDirectory()
            {
                Path = VosDiskPaths.BasePacksPath_AppData,
                LayerName = DefaultMountOptions.AppDataLayerName,
                MountOptions = new MountOptions()
                {
                    IsReadOnly = true, // Base = readonly
                    ReadPriority = -50,
                    WritePriority = 10100, // Must have permissions
                },
            });
#endif

            #endregion

            #region UserPacks

            packagePaths.Add(VosDiskPaths.AppData);
            packageDirectories.Add(new PackageDirectory()
            {
                Path = VosDiskPaths.AppData,
                LocationName = VosStoreNames.AppData,
                MountOptions = new MountOptions()
                {
                    TryCreateIfMissing = false,
                    IsReadOnly = true, // Shouldn't have permissions
                    ReadPriority = 100,
                    WritePriority = 1000,  // Shouldn't have permissions, unless user manually enabled this
                },
            });

            if (useVar)
            {
                packagePaths.Add(VosDiskPaths.VarData);
                {
                    PackageDirectory pd = new PackageDirectory()
                    {
                        Path = VosDiskPaths.VarData,
                        LocationName = VosStoreNames.VarData,
                        MountOptions = new MountOptions()
                        {
                            TryCreateIfMissing = true,
                            IsReadOnly = false, // May not have permissions
                            ReadPriority = 200,
                            WritePriority = 11000, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

#if ENABLE_APPDATA_FALLBACK
            packageDirectories.Add(new PackageDirectory()
            {
                Path = VosDiskPaths.UserPacksPath_AppData,
                LayerName = DefaultMountOptions.AppDataLayerName,
                MountOptions = new MountOptions()
                {
                    TryCreateIfMissing = true,
                    IsReadOnly = false, // Must have permissions
                    ReadPriority = 300,
                    WritePriority = 100, // Must have permissions
                },
            });
#endif

            #endregion

            #region Default Dir (changeable)

            if (!StringX.IsNullOrWhiteSpace(VosConfiguration.CustomBaseDir))
            {
                if (packagePaths.Contains(VosConfiguration.CustomBaseDir))
                {
                    l.Trace("VosConfiguration.CustomBaseDir is already a package directory.");
                }
                else
                {
                    packagePaths.Add(VosConfiguration.CustomBaseDir);
                    PackageDirectory pd = new PackageDirectory()
                    {
                        Path = VosConfiguration.CustomBaseDir,
                        LocationName = VosStoreNames.CustomBase,
                        MountOptions = new MountOptions()
                        {
                            TryCreateIfMissing = true,
                            IsReadOnly = true,
                            ReadPriority = 2000,
                            WritePriority = 20000, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

            if (!StringX.IsNullOrWhiteSpace(VosConfiguration.CustomDataDir))
            {
                if (packagePaths.Contains(VosConfiguration.CustomDataDir))
                {
                    l.Trace("VosConfiguration.CustomDataDir is already a package directory.");
                }
                else
                {
                    packagePaths.Add(VosConfiguration.CustomDataDir);
                    PackageDirectory pd = new PackageDirectory()
                    {
                        Path = VosConfiguration.CustomDataDir,
                        LocationName = VosStoreNames.CustomData,
                        MountOptions = new MountOptions()
                        {
                            TryCreateIfMissing = true,
                            IsReadOnly = false, // May not have permissions
                            ReadPriority = 3000,
                            WritePriority = 12000, // May not have permissions
                        },
                    };

                    packageDirectories.Add(pd);
                }
            }

            #endregion

            var sb = new StringBuilder();

            sb.AppendLine(packageDirectories.Count + " package directories: ");
            foreach (var dir in packageDirectories)
            {
                sb.AppendLine(" - " + dir.Path);
            }
            l.Debug(sb.ToString());
        }

        #endregion

        #region Mounts

        public static bool DefaultMountAppBase = true;
        public static bool DefaultMountActiveDataAtAppBase = true;

        public static bool DefaultMountAppData = true;
        public static bool DefaultMountActiveDataAtAppData = true;

        public static bool DefaultMountVarBase = true;
        public static bool DefaultMountActiveDataAtVarBase = true;

        public static bool DefaultMountVarData = true;
        public static bool DefaultMountActiveDataAtVarData = true;

        static int customMountPriority = 0;
        public void MountCustom(string dir)
        {
            foreach (var mount in CreateCustomMount(dir, customMountPriority--))
            {
                mount.IsEnabled = true;
            }
        }

        public virtual List<Mount> CreateCustomMount(string dir, int priorityModifier = 0)
        {
            var vosMounts = new List<Mount>();
            vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.CustomData], VosConfiguration.CustomDataDir.AsFileReference(), null, VosStoreNames.CustomData, false, new MountOptions()
            {
                MountAtStartup = true,
                MountOnDemand = true,
                IsExclusive = true,
                IsReadOnly = false,

                // (Exclusive, so priority shouldn't matter:)
                ReadPriority = 1000,
                WritePriority = 1000,
            }));

            vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosApp.Instance.Stores[VosStoreNames.CustomData], null, VosStoreNames.CustomData, false, new MountOptions()
            {
                MountAtStartup = true,
                MountOnDemand = true,
                IsReadOnly = false,

                ReadPriority = 1000 + priorityModifier,
                WritePriority = -20000 + priorityModifier,
            }));

            return vosMounts;
        }

        public List<Mount> MountAppVarData(string appName, string companyName = null, bool enable = true, bool mountToActiveData = true, bool isReadOnly = true)
        {
            // TODO: Make non readonly by default
            var appVarData = VosDiskPaths.AppVarDataDir(appName, companyName);

            var storeName = VosStoreNames.VarData + "-" + appName;

            List<Mount> vosMounts = new List<Mount>();

            {
                vosMounts.Add(new Mount(VosApp.Instance.Stores[storeName], appVarData.AsFileReference(), null, storeName, enable, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = isReadOnly,
                }));
            }

            if (mountToActiveData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, appVarData.AsFileReference(), null, storeName, enable, new MountOptions()
                {
                    IsReadOnly = isReadOnly,
                    ReadPriority = 90,
                    WritePriority = -21000,
                }));
            }

            return vosMounts;
        }

        public virtual List<Mount> GetDefaultMounts()
        {
            List<Mount> vosMounts = new List<Mount>();
            #region OLD

            //#region User Packs

            //packageDirectories.Add(new PackageDirectory()
            //{
            //    Path = VosDiskPaths.UserPacksPath_CommonAppData,
            //    LayerName = DefaultMountOptions.CommonAppDataLayerName,
            //    MountOptions = new MountOptions()
            //    {
            //        IsReadOnly = false, // May not have permissions
            //        ReadPriority = 100,
            //        WritePriority = -500, // May not have permissions
            //    },
            //});

            //packageDirectories.Add(new PackageDirectory()
            //{
            //    Path = VosDiskPaths.UserPacksPath_CommonAppData,
            //    LayerName = DefaultMountOptions.CommonAppDataLayerName,
            //    MountOptions = new MountOptions()
            //    {
            //        IsReadOnly = false, // May not have permissions
            //        ReadPriority = -100,
            //        WritePriority = -100, // May not have permissions
            //    },
            //});

            //packageDirectories.Add(new PackageDirectory()
            //{
            //    Path = VosDiskPaths.UserPacksPath_CommonAppData,
            //    LayerName = DefaultMountOptions.CommonAppDataLayerName,
            //    MountOptions = new MountOptions()
            //    {
            //        IsReadOnly = false, // May not have permissions
            //        ReadPriority = 100,
            //        WritePriority = -500, // May not have permissions
            //    },
            //});


            //packageDirectories.Add(new PackageDirectory()
            //{
            //    Path = VosDiskPaths.BasePacksPath_AppDir,
            //    LayerName = DefaultMountOptions.AppDirLayerName,
            //    MountOptions = DefaultMountOptions.Base_AppDirMountOptions,
            //});

            //#endregion
            #endregion

            #region Base Data

            #region AppBase

            if (DefaultMountAppBase)
            {
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.AppBase], VosDiskPaths.AppBase.AsFileReference(), null, VosStoreNames.AppBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = true,
                    //ReadPriority = -1000,
                    //WritePriority = -100,
                }));
            }

            if (DefaultMountActiveDataAtAppBase)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.AppBase.AsFileReference(), null, VosStoreNames.AppBase, false, new MountOptions()
                {
                    IsReadOnly = true,
                    ReadPriority = -1000,
                    WritePriority = -100,
                }));
            }
            #endregion

            #region VarBase

            if (DefaultMountVarBase)
            {
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.VarBase], VosDiskPaths.VarBase.AsFileReference(), null, VosStoreNames.VarBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = true,
                    //ReadPriority = -600,
                    //WritePriority = -1200,
                }));
            }

            if (DefaultMountActiveDataAtVarBase)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.VarBase.AsFileReference(), null, VosStoreNames.VarBase, false, new MountOptions()
                {
                    IsReadOnly = false, // RECENTCHANGE - this was true??
                    ReadPriority = -500,
                    WritePriority = -50,
                }));
            }

            #endregion

#if ENABLE_APPDATA_FALLBACK
            vosMounts.Add(new Mount(VosApp.Instance.Base, VosDiskPaths.BaseDataPath_AppData.AsFileReference(), DefaultMountOptions.AppDataLayerName, false, new MountOptions()
            {
                IsReadOnly = false,
                ReadPriority = -500,
                WritePriority = -1000,
            }));
#endif
            #endregion

            #region Data

            #region AppData

            if (DefaultMountAppData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.AppData], VosDiskPaths.AppData.AsFileReference(), null, VosStoreNames.AppBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = false, // Might be read-only on disk
                    //ReadPriority = -1000,
                    //WritePriority = -100,
                }));
            }

            if (DefaultMountActiveDataAtAppData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.AppData.AsFileReference(), null, VosStoreNames.AppData, false, new MountOptions()
                {
                    IsReadOnly = false,  // Might be read-only on disk
                    ReadPriority = 50,
                    WritePriority = 50,
                }));
            }

            #endregion

            #region VarData

            if (DefaultMountVarData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.VarData], VosDiskPaths.VarData.AsFileReference(), null, VosStoreNames.VarBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = false,
                    //ReadPriority = -900, // Shouldn't matter for IsExclusive
                    //WritePriority = 1000,  // Shouldn't matter for IsExclusive
                }));
            }

            if (DefaultMountActiveDataAtVarData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.VarData.AsFileReference(), null, VosStoreNames.VarData, false, new MountOptions()
                {
                    IsReadOnly = false,
                    ReadPriority = 100,
                    WritePriority = -20000, // Preferred write location
                }));
            }
            #endregion

            #endregion

            #region Custom Path

            if (!StringX.IsNullOrWhiteSpace(VosConfiguration.CustomBaseDir))
            {
            }

            #region Data

            if (!StringX.IsNullOrWhiteSpace(VosConfiguration.CustomDataDir))
            {
                var customMounts = CreateCustomMount(VosConfiguration.CustomDataDir);
                foreach (var m in customMounts)
                {
                    vosMounts.Add(m);
                }

            }
            #endregion


            #endregion

            #region Local

            //vosMounts.Add(new Mount(VosApp.Instance.Local, VosDiskPaths.VarData.AsFileReference(), null, VosLocationNames.VarData, false, new MountOptions()
            //{
            //    TryCreateIfMissing = false,
            //    IsReadOnly = false,
            //    ReadPriority = -1000,
            //    WritePriority = -1000,
            //}));

#if ENABLE_APPDATA_FALLBACK
            vosMounts.Add(new Mount(VosApp.Instance.Local, VosDiskPaths.UserDataPath_AppData.AsFileReference(), null, DefaultMountOptions.AppDataLayerName, false, new MountOptions()
            {
                TryCreateIfMissing = false,
                IsReadOnly = false,
                ReadPriority = -500,
                WritePriority = -500,
            }));
#endif

            #endregion

            #region Experimental

#if RKV
                string dbName = "Rkv";

                vosMounts.Add(new Mount(Vos.Default[VosPaths.DBsPath]["RKV"], RkvReference.Root, null, DefaultMountOptions.DataDbLayerName, false, new MountOptions()
                {
                    TryCreateIfMissing = true,
                    IsReadOnly = false,
                    ReadPriority = 100,
                    WritePriority = 100,
                    ProviderType = typeof(RkvOBaseProvider),
                    ConnectionReference = VosDiskPaths.Default.Databases.AsFileReference().GetChild(dbName),
                }));
#endif
            #endregion

            //#region OLD Mount Package dirs: NO!, just Data dirs.

            ////// AppDir
            ////vosMounts.Add(new Mount(VosApp.Instance.Packages, VosDiskPaths.BaseDataPath_AppDir.AsFileReference(), null, "AppDir", true, DefaultMountOptions.AppDirMountOptions));

            ////// AppData
            ////vosMounts.Add(new Mount(VosApp.Instance.Packages, VosDiskPaths.BaseDataPath_AppData.AsFileReference(), null, "AppData", false, new MountOptions()
            ////{
            ////    IsReadOnly = false,
            ////    ReadPriority = 100,
            ////    WritePriority = 100,
            ////}));

            //// CommonAppData // TODO: Enable user data?
            ////vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.UserDataPath_CommonAppData.AsFileReference(), null, DefaultMountOptions.CommonAppDataLayerName, false, new MountOptions()
            ////{
            ////    IsReadOnly = false,
            ////    ReadPriority = -100,
            ////    WritePriority = -1000,
            ////}));

            //#endregion

            // TODO: AppUserDir? @"d:\valor\PackagesUser"; // FIXME HARDCODE HARDPATH

            //Vos.Default.Mounts.Mount("AppData", PackagePath, new FsReference(VosDiskPaths.DataPath_AppData));
            //Vos.Default.Mounts.Mount("CommonAppData", PackagePath, new FsReference(VosDiskPaths.DataPath_CommonAppData));

            //#region Data Packs OLD

            ////PackMountOptions = new MountOptions()
            ////{
            ////    IsReadOnly = true,
            ////};

            ////foreach (var pack in Directory.GetFiles(DataPacksPath_AppDir))
            ////{
            ////    VosMountManager.Instance.Mount("AppDir", PackagePath, new FileReference(VosDiskPaths.DataPath_AppDir), new MountOptions
            ////    {
            ////        IsReadOnly = true,
            ////    });
            ////}

            //#endregion

#if true
            {
                //StringBuilder sb = new StringBuilder();
                l.Info("Default VOS Mounts: ");
                foreach (var m in vosMounts)
                {
                    l.Info("[mount] " + m.Vob + " ==> " + m.Root.ToString() + " [R: " + m.MountOptions.ReadPriority + ", W: " + m.MountOptions.WritePriority + "]");
                    //sb.Append(" - ");
                    //sb.Append(m.Vob);
                    //sb.Append(" - ");
                    //sb.AppendLine(m.Root.ToString());
                }
                //l.Info("Default VOS Mounts: " + Environment.NewLine + sb.ToString());
            }
#endif
            return vosMounts;
        }

        #endregion

        #region DB

        public Vob DBs
        {
            get
            {
                if (dbs == null)
                {
                    dbs = VBase.Default[VosPaths.DBsPath];
                }
                return dbs;
            }
        }
        private Vob dbs;

        #endregion

        public Vob Root
        {
            get
            {
                return VBase.Default.Root;
            }
        }
        #region ActiveData

        public bool HasActiveData { get { return activeData != null; } }
        public Vob ActiveData
        {
            get
            {
                if (activeData == null)
                {
                    activeData = VBase.Default[VosPaths.ActiveDataPath];
                }
                return activeData;
            }
        }
        private Vob activeData;

        #endregion

        #region AppData

        public Vob AppData
        {
            get
            {
                if (appData == null)
                {
                    appData = VBase.Default["AppData"];
                }
                return appData;
            }
        }
        private Vob appData;

        #endregion

        #region Assets

        public Vob Assets
        {
            get
            {
                if (assets == null)
                {
                    assets = ActiveData[AssetsSubpath]; // RECENTCHANGE
                    //assets = ActiveData;
                }
                return assets;
            }
        }
        private Vob assets;

        public static string AssetsSubpath
        {
            get
            {
                return assetsSubpath ?? AssetsSubpathDefault;
            }
            set {
                assetsSubpath = value;
            }
        }
        private static string assetsSubpath;

#if ASSETS_SUBPATH
        public const string AssetsSubpathDefault = "Assets";
#else
        public const string AssetsSubpathDefault = "";
#endif

        public static string[] AssetsSubpathChunks
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(AssetsSubpath))
                {
                    return new string[] { AssetsSubpath };
                }
                else
                {
                    return new string[] { };
                }
            }
        }

        //private static readonly string AssetsLocation = "Assets"; - Won't work, since Packages[packageName][assetSubpath] is used.  Need PackageAssets[packageName] or Package[packageName][assetsLocation][assetSubpath]


        public static Vob GetAssetFolder<AssetType>()
        {
            // TODO: Filtered Vob
            return V.Assets[AssetPaths.GetAssetTypeFolder(typeof(AssetType))];
        }

        public static Vob GetAssetFolder(Type assetType)
        {
            // TODO: Filtered Vob
            return V.Assets[AssetPaths.GetAssetTypeFolder(assetType)];
        }

        public static VobHandle<AssetType> GetAsset<AssetType>(string name)
            where AssetType : class, new()
        {
            return V.Assets[AssetPaths.GetAssetTypeFolder(typeof(AssetType)), name];
        }
        public static Vob GetAsset(Type assetType, string name)
        {
            return V.Assets[AssetPaths.GetAssetTypeFolder(assetType), name];
        }

        public static string GetAssetPath(string name, Type assetType)
        {
            return V.Assets[AssetPaths.GetAssetTypeFolder(assetType), name].Path;
        }

        public static string NameToPathForType(string name, Type type) // 
        {
            //return AssetPath.PathFromName(name, type); - Normal default
            return VosApp.GetAssetPath(name, type);
        }

        #endregion

        public Vob PackageStores
        {
            get
            {
                if (packageStores == null)
                {
                    packageStores = VBase.Default[VosPaths.PackageStores];
                }
                return packageStores;
            }
        }
        private Vob packageStores;

        public Vob Stores
        {
            get
            {
                if (stores == null)
                {
                    stores = VBase.Default[VosPaths.Stores];
                }
                return stores;
            }
        }
        private Vob stores;

        #region Packages

        public Vob Packages
        {
            get
            {
                if (packages == null)
                {
                    packages = VBase.Default[VosPaths.Packages];
                }
                return packages;
            }
        }
        private Vob packages;

        public Vob PackageMounts
        {
            get
            {
                if (packageMounts == null)
                {
                    packageMounts = VBase.Default[VosPaths.PackageMounts];
                }
                return packageMounts;
            }
        }
        private Vob packageMounts;

        #endregion

        //#region Base Data Storage

        //public Vob Base
        //{
        //    get
        //    {
        //        if (baseData == null)
        //        {
        //            baseData = Vos.Default[VosPaths.BaseDataPath];
        //        }
        //        return baseData;
        //    }
        //} private Vob baseData;

        //#endregion

        #region Local Data Storage

        public Vob Local
        {
            get
            {
                if (local == null)
                {
                    local = VBase.Default[VosPaths.LocalPath];
                }
                return local;
            }
        }
        private Vob local;

        #endregion

        #region Local Data Storage

        public Vob Accounts
        {
            get
            {
                if (accounts == null)
                {
                    accounts = Local[VosPaths.AccountsPath];
                }
                return accounts;
            }
        }
        private Vob accounts;

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion

    }

    public class StoreMounter
    {
        public static IEnumerable<IReference> Locations
        {
            get
            {
                yield return VosDiskPaths.UserSharedStoresRoot.AsFileReference();
            }
        }
        public static IEnumerable<IReference> GlobalLocations
        {
            get
            {
#if UNITY
                yield break;
#else
                yield return VosDiskPaths.GlobalSharedStoresRoot.AsFileReference();

#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="createIfMissing"></param>
        /// <param name="mountAsStoreName"></param>
        /// <param name="useGlobal">FUTURE: If true, use or create the global store.  If false, use or create the user store.  If null, load either or create user if none exists.</param>
        /// <returns></returns>
        public static Mount Mount(string storeName, bool createIfMissing = true, string mountAsStoreName = null, bool? useGlobal = null)
        {
            if (mountAsStoreName == null) mountAsStoreName = storeName;

            Mount mount = null;

            IEnumerable<IReference> locations;

            if (useGlobal.HasValue)
            {
                locations = useGlobal.Value ? GlobalLocations : Locations;
            }
            else
            {
                locations = Locations.Concat(GlobalLocations).Distinct();
            }

            foreach (var location in locations)
            {
                var storeLocation = location.GetChildSubpath(storeName);
                var storeMetaLocation = storeLocation.GetChildSubpath(StoreMetadata.DefaultName);
                var hStore = storeMetaLocation.GetHandle<StoreMetadata>();
                var metadata = hStore.Object;
                if (metadata != null)
                {
                    l.Info("[store] Found store at " + location);

                    mount = new Mount(V.Stores[mountAsStoreName], storeLocation, store: storeName, enable: true, mountOptions: new MountOptions()
                    {
                        IsExclusive = true,
                    });
                }
            }

            if (mount == null && createIfMissing)
            {
                foreach (var location in locations)
                {
                    try
                    {
                        var storeLocation = location.GetChildSubpath(storeName);
                        var storeMetaLocation = storeLocation.GetChildSubpath(StoreMetadata.DefaultName);
                        var hStore = storeMetaLocation.GetHandle<StoreMetadata>();
                        var metadata = new StoreMetadata();

                        hStore.Object = metadata;
                        hStore.Save();

                        mount = new Mount(V.Stores[mountAsStoreName], storeLocation, store: storeName, enable: true, mountOptions: new MountOptions()
                        {
                            IsExclusive = true,
                        });
                        break;
                    }
                    catch (Exception)
                    {
                        l.Error("Failed to create mount at " + location);
                    }
                }
            }

            return mount;
        }

        //public static Mount Mount(LocalFileReference fileReference, bool createIfMissing = true, string mountAsStoreName = null, bool? useGlobal = null)
        //{

        //    return null;

        //}

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

    public static class VobVosAppExtensions
    {
        public static string GetSubPathOfAncestor(this Vob vob, Vob potentialAncestor)
        {
            var result = vob.GetSubPathChunksOfAncestor(potentialAncestor);
            if (result == null) return null;
            return result.Aggregate((x, y)=> x + VosPath.Separator + y);
        }

        public static IEnumerable<string> GetSubPathChunksOfAncestor(this Vob vob, Vob potentialAncestor)
        {
            List<string> subPathChunks = new List<string>();

            for (var parent = vob.Parent; parent.Parent != parent; parent = parent.Parent)
            {
                if (parent == potentialAncestor) return ((IEnumerable<string>)subPathChunks).Reverse();
            }
            return null;
        }

        public static string GetPackageStoreSubPath(this Vob vob)
        {
            return vob.GetPackageStoreSubPathChunks().ToSubPath();
        }
        
        public static IEnumerable<string> GetPackageStoreSubPathChunks(this Vob vob)
        {
            var subPathChunks = vob.GetSubPathChunksOfAncestor(V.Archives).ToList();
            if (subPathChunks == null || subPathChunks.Count <= 1) return null;
            return subPathChunks.Skip(2);
        }

    }
    
}

//VosMountManager.Instance.Mount("/fs", "file:///");
//VosMountManager.Instance.Mount("/UserData", "file:///"); // TODO: OS-specific User dir
//VosMountManager.Instance.Mount("/AppData", "file:///"); // TODO: App dir

//VosMountManager.Instance.Mount(new MountOptions
//{
//    MountPath = "/fs",
//    ConnectionString = "",
//});
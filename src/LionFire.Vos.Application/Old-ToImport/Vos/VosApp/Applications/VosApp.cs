#if OLD
//#define ASSETS_SUBPATH // Prefer this off?  TODO - make sure this works for packages

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Assets;
using LionFire.Dependencies;
using LionFire.Execution;
using LionFire.Persistence;
using LionFire.Structures;
using LionFire.Vos.Mounts;
using LionFire.Vos.Packages;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos.VosApp.Old
{
    public class VosApp : IInitializable3
    {
        //VosMountManager.Instance.Mount("/fs", "file:///");
        //VosMountManager.Instance.Mount("/UserData", "file:///"); // TODO: OS-specific User dir
        //VosMountManager.Instance.Mount("/AppData", "file:///"); // TODO: App dir

        //VosMountManager.Instance.Mount(new MountOptions
        //{
        //    MountPath = "/fs",
        //    ConnectionString = "",
        //});

        #region (Static) Instance Accessor

        public static VosApp Instance => ManualSingleton<VosApp>.GuaranteedInstance;

        #endregion

        #region Ontology

        public RootVob Root { get; }
            // => VosOBus.Instance.DefaultVBase.Root;
        //public VBase VBase => VosOBus.Instance.DefaultVBase;

        #endregion

        #region Configuration

        private VosAppOptions options;

        #endregion

        #region Members

        //public PackageMounter PackageMounter { get; protected set; } = new PackageMounter(); // OLD

        #endregion

        #region Construction

        public VosApp(RootVob root,  VosAppOptions options = null)
        {
            this.Root = root;
            this.options = options ?? new VosAppOptions();
        }

        private bool isInitialized;
        public async Task<object> Initialize()
        {
            VosContext.DefaultRoot = V.ActiveData;

            if (isInitialized)
            {
                return Task.FromResult<object>(null);
            }

            // ENH: Consider mixing in the in-class Initialize_ methods with the batch of IEnumerable<IInitializerFor<VosApp>> to allow them to sort it out
            await DependencyContext.Current.GetService<IEnumerable<IInitializerFor<VosApp>>>().InitializeAll(this).ConfigureAwait(false);

            Initialize_MountDefaultMounts();

            Initialize_Packages();

            isInitialized = true;
            return Task.FromResult<object>(null);
        }

        public void Initialize_MountDefaultMounts()
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

        public void Initialize_Packages()
        {
            if (options.UsePackages)
            {
                InitPackageDirectories();

                PackageMounter.MountAllPackages();

                //this.PackageMounter.InitVos();
            }
        }

        #endregion

        #region Packages

        public virtual void InitPackageDirectories()
        {
            bool useVar = VosDiskPaths.AppBase != VosDiskPaths.VarBase;

            var packagePaths = new List<string>();  // Used to detect multiple registrations of the same path

            var packageDirectories = PackageMounter.PackageDirectories;

            #region BasePacks

            packagePaths.Add(VosDiskPaths.AppBase);
            {
                var pd = new PackageDirectory()
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
                    var pd = new PackageDirectory()
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

            if (!string.IsNullOrWhiteSpace(VosConfiguration.CustomBaseDir))
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

            if (!string.IsNullOrWhiteSpace(VosConfiguration.CustomDataDir))
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
        private static int customMountPriority = 0;
        public void MountCustom(string dir)
        {
            foreach (var mount in CreateCustomMount(dir, customMountPriority--))
            {
                mount.IsEnabled = true;
            }
        }

        public virtual List<Mount> CreateCustomMount(string dir, int priorityModifier = 0)
        {
            var vosMounts = new List<Mount>
            {
                new Mount(VosApp.Instance.Stores[VosStoreNames.CustomData], VosConfiguration.CustomDataDir.ToFileReference(), null, VosStoreNames.CustomData, false, new MountOptions()
                {
                    MountAtStartup = true,
                    MountOnDemand = true,
                    IsExclusive = true,
                    IsReadOnly = false,

                    // (Exclusive, so priority shouldn't matter:)
                    ReadPriority = 1000,
                    WritePriority = 1000,
                }),

                new Mount(VosApp.Instance.ActiveData, VosApp.Instance.Stores[VosStoreNames.CustomData], null, VosStoreNames.CustomData, false, new MountOptions()
                {
                    MountAtStartup = true,
                    MountOnDemand = true,
                    IsReadOnly = false,

                    ReadPriority = 1000 + priorityModifier,
                    WritePriority = -20000 + priorityModifier,
                })
            };

            return vosMounts;
        }

        public List<Mount> MountAppVarData(string appName, string companyName = null, bool enable = true, bool mountToActiveData = true, bool isReadOnly = true)
        {
            // TODO: Make non readonly by default
            var appVarData = VosDiskPaths.AppVarDataDir(appName, companyName);

            var storeName = VosStoreNames.VarData + "-" + appName;

            List<Mount> vosMounts = new List<Mount>();

            {
                vosMounts.Add(new Mount(VosApp.Instance.Stores[storeName], appVarData.ToFileReference(), null, storeName, enable, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = isReadOnly,
                }));
            }

            if (mountToActiveData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, appVarData.ToFileReference(), null, storeName, enable, new MountOptions()
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
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.AppBase], VosDiskPaths.AppBase.ToFileReference(), null, VosStoreNames.AppBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = true,
                    //ReadPriority = -1000,
                    //WritePriority = -100,
                }));
            }

            if (DefaultMountActiveDataAtAppBase)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.AppBase.ToFileReference(), null, VosStoreNames.AppBase, false, new MountOptions()
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
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.VarBase], VosDiskPaths.VarBase.ToFileReference(), null, VosStoreNames.VarBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = true,
                    //ReadPriority = -600,
                    //WritePriority = -1200,
                }));
            }

            if (DefaultMountActiveDataAtVarBase)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.VarBase.ToFileReference(), null, VosStoreNames.VarBase, false, new MountOptions()
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
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.AppData], VosDiskPaths.AppData.ToFileReference(), null, VosStoreNames.AppBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = false, // Might be read-only on disk
                    //ReadPriority = -1000,
                    //WritePriority = -100,
                }));
            }

            if (DefaultMountActiveDataAtAppData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.AppData.ToFileReference(), null, VosStoreNames.AppData, false, new MountOptions()
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
                vosMounts.Add(new Mount(VosApp.Instance.Stores[VosStoreNames.VarData], VosDiskPaths.VarData.ToFileReference(), null, VosStoreNames.VarBase, false, new MountOptions()
                {
                    IsExclusive = true,
                    IsReadOnly = false,
                    //ReadPriority = -900, // Shouldn't matter for IsExclusive
                    //WritePriority = 1000,  // Shouldn't matter for IsExclusive
                }));
            }

            if (DefaultMountActiveDataAtVarData)
            {
                vosMounts.Add(new Mount(VosApp.Instance.ActiveData, VosDiskPaths.VarData.ToFileReference(), null, VosStoreNames.VarData, false, new MountOptions()
                {
                    IsReadOnly = false,
                    ReadPriority = 100,
                    WritePriority = -20000, // Preferred write location
                }));
            }
            #endregion

            #endregion

            #region Custom Path

            if (!string.IsNullOrWhiteSpace(VosConfiguration.CustomBaseDir))
            {
            }

            #region Data

            if (!string.IsNullOrWhiteSpace(VosConfiguration.CustomDataDir))
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
                    l.Info("[mount] " + m.Vob + " ==> " + m.Target.ToString() + " [R: " + m.MountOptions.ReadPriority + ", W: " + m.MountOptions.WritePriority + "]");
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
                    dbs = VBase[VosPaths.DBsPath];
                }
                return dbs;
            }
        }
        private Vob dbs;

        #endregion

        #region ActiveData

        public bool HasActiveData => activeData != null;
        public Vob ActiveData
        {
            get
            {
                if (activeData == null)
                {
                    activeData = VBase[VosPaths.ActiveDataPath];
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
                    appData = VBase["AppData"];
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
        private IVob assets;

        public static string AssetsSubpath
        {
            get => assetsSubpath ?? AssetsSubpathDefault;
            set => assetsSubpath = value;
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

        public static IReadHandle<AssetType> GetAsset<AssetType>(string name)
            where AssetType : class, new() => V.Assets[AssetPaths.GetAssetTypeFolder(typeof(AssetType)), name];

        public static Vob GetAsset(Type assetType, string name) => V.Assets[AssetPaths.GetAssetTypeFolder(assetType), name];

        public static string GetAssetPath(string name, Type assetType) => V.Assets[AssetPaths.GetAssetTypeFolder(assetType), name].Path;

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
                    packageStores = VBase[VosPaths.PackageStores];
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
                    stores = VBase[VosPaths.Stores];
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
                    packages = VBase[VosPaths.Packages];
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
                    packageMounts = VBase[VosPaths.PackageMounts];
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
                    local = VBase[VosPaths.LocalPath];
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
}
#endif

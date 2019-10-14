using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using System.IO;
using System.Reflection;
using LionFire.Serialization;
using LionFire.ObjectBus;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace LionFire.Assets
{
    // TODO: REDESIGN this into a Vos add-on, separate from Assets

    public class AssetContext : IDisposable
    {
        public const string DefaultAssetsSubpath = "Packages";

        #region Static

        //public static IEnumerable<string> PackagePaths { get; set; }

        public static AssetContext Global;

        public static AssetContext Current
        {
            get
            { return current; }
            set
            {
                if (current == value) return;
                AssetContext oldCurrent = Current;
                current = value;
                current.Parent = oldCurrent;
            }
        }
        private static AssetContext current;

        public static AssetContext EffectiveContext
        {
            get { return AssetContext.Current ?? AssetContext.Global; }
        }

        static AssetContext()
        {
            Global = new AssetContext();
            var packageParentDirectories = new MultiBindableCollection<string>();

            string rootLocation = LionFireEnvironment.Directories.AppDir;

            

            Global.DefaultPackageDirectory = Path.Combine(Path.GetDirectoryName(rootLocation), DefaultAssetsSubpath);
            Global.DefaultUserPackageDirectory = Path.Combine(LionFireEnvironment.Directories.CompanyLocalAppDataPath + @"\" + Assembly.GetEntryAssembly().GetName(), DefaultAssetsSubpath);
            packageParentDirectories.Add(Global.DefaultPackageDirectory);
            packageParentDirectories.Add(Global.DefaultUserPackageDirectory);
            Global.PackageParentDirectories = packageParentDirectories;

            Current = Global;
        }

        #endregion

        internal Dictionary<string, object> instancesLoadedByPath = new Dictionary<string, object>();
        internal object instancesLoadedByPathLock = new object();

        public AssetContext Parent;

        public IList<Package> Packages
        {
            get
            {
                if (packages == null)
                {
                    //packages = new List<Package>();
                    LoadAllPackages();
                }
                return packages;
            }
            set
            {
                packages = value;
            }
        }
        private IList<Package> packages;

        public IEnumerable<AssetReference<AssetType>> GetAssets<AssetType>()
            where AssetType : class, IAsset
        {
            foreach (Package package in
#if AOT
			         (IEnumerable)
#endif
                     Packages)
            {
                foreach (AssetReference<AssetType> asset in
#if AOT
				         (IEnumerable)
#endif
                         package.Assets.GetAssets<AssetType>())
                {
                    yield return asset;
                }
            }
        }

        private void LoadAllPackages() // OLD? Should not have file io?
        {
            List<Package> detectedPackages = new List<Package>();
            foreach (string packageParentDirectory in (IEnumerable)PackageParentDirectories)
            {
                if (!Directory.Exists(packageParentDirectory)) continue;
                foreach (string potentialPackageDirectory in Directory.GetDirectories(packageParentDirectory))
                {
                    try
                    {
                        string potentialPackageName = Path.GetFileName(potentialPackageDirectory); // UNTESTED - does this work as intended?

                        string packagePathWithoutExtension = Path.Combine(potentialPackageDirectory, potentialPackageName);

                        throw new NotImplementedException("TODO: use FSOBase instead of serializationfacility");
#if TOPORT
                        object potentialPackage;
                        try
                        {
                            potentialPackage = SerializationFacility.Deserialize(packagePathWithoutExtension);
                        }
                        catch (Exception ex)
                        {
                            l.Warn("Failed to load package object at: " + packagePathWithoutExtension + " (extension omitted.)  This folder does not appear to contain a valid package.  Exception: " + ex);
                            continue;
                        }
                        Package package = potentialPackage as Package;
                        if (package != null)
                        {
                            l.Info("Loaded package: " + package.AssetTypePath);
                            package.PackageDirectory = potentialPackageDirectory;
                            detectedPackages.Add(package);
                        }
                        else
                        {
                            l.Trace("No package found in directory: " + potentialPackageDirectory);
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        l.Error("Exception when attempting to load package in: " + potentialPackageDirectory + " Exception: " + ex);
                        continue;
                    }
                }
            }
            this.packages = detectedPackages;

            //string defaultPath = Path.Combine(Package.PackageDirectory, defaultDirectory);

            //List<IAssetReference> assets = new List<IAssetReference>();

            //foreach (string filepath in Directory.GetFiles(defaultPath))
            //{
            //    if (loadAssets || verifyAssetType || loadFullAssetIDs)
            //    {
            //        l.Info("UNTESTED: Loading assets");
            //        AssetType asset;
            //        object deserializedObject = SerializationFacility.Deserialize(filepath);
            //        asset = deserializedObject as AssetType;
            //        if (asset == null)
            //        {
            //            l.Info("Unexpected asset of type '" +
            //                (deserializedObject == null ? "null" : deserializedObject.GetType().Name) +
            //                "' in default asset directory for type '" + typeof(AssetType) + "'.");
            //            continue;
            //        }
            //        if (loadAssets)
            //        {
            //            assets.Add(new AssetReference<AssetType>(asset));
            //        }
            //        else
            //        {
            //            assets.Add(new AssetReference<AssetType>(asset.ID));
            //        }
            //    }
            //    else
            //    {
            //        l.Info("UNTESTED: Finding asset IDs - is assetPath right?");

            //        string lastIndexOfString = !String.IsNullOrEmpty(defaultDirectory) ? defaultDirectory : Package.PackageDirectory;

            //        string assetPath = filepath.Substring(filepath.LastIndexOf(lastIndexOfString) + lastIndexOfString.Length);
            //        assetPath = Path.GetFileNameWithoutExtension(assetPath);
            //        assets.Add(new AssetReference<AssetType>(assetPath));
            //    }

            //}
            //return assets;
        }

        public AssetContext() { }
        public AssetContext(IList<Package> packages)
        {
            this.Packages = packages;
        }

        /// <summary>
        /// Package assigned to newly created.
        /// </summary>
        public Package DefaultCreatePackage { get; set; }

        #region Saving Context

        /// <summary>
        /// Used for saving assets when there is no Package specified.
        /// </summary>
        public Package DefaultSavePackage { get; set; }

        /// <summary>
        /// Used for saving new packages
        /// </summary>
        public string DefaultPackageDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Used for saving new packages in user space
        /// </summary>
        public string DefaultUserPackageDirectory { get; set; }

        #endregion

        #region Loading Context

        /// <summary>
        /// Directories in which packages are found (i.e. each auto-locatable package's parent directory.)
        /// </summary>
        public IEnumerable<string> PackageParentDirectories;

        /// <summary>
        /// Directories of actual packages
        /// </summary>
        public IEnumerable<string> PackageDirectories
        {
            get { return Packages.Select(p => p.PackageDirectory).ToList(); }
        }

        #endregion

        #region Methods

        internal string GetPathForPackage(string packageName)
        {
            foreach (string packagePath in (IEnumerable)PackageParentDirectories)
            {
                string combined = Path.Combine(packagePath, packageName);
                if (Directory.Exists(combined))
                {
                    return combined;
                }
            }
            return null;
        }

        internal string GetPathForAsset(string assetPath)
        {
            foreach (string packagePath in (IEnumerable)PackageDirectories)
            {
                string combined = Path.Combine(packagePath, assetPath);
                throw new NotImplementedException("TODO: use FSOBase instead of serializationfacility");
#if TOPORT
                if (SerializationFacility.Exists(combined))
                {
                    return combined;
                }
#endif
            }
            return null;
        }

#endregion

        public void Dispose()
        {
            if (AssetContext.Current == this)
            {
                AssetContext.Current = this.Parent;
            }
        }

        private static ILogger l = Log.Get();

    }
}

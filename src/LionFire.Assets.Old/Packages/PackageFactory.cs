//#define TRACE_CREATE
#define DEPLOY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.ObjectBus;
using System.Collections.Concurrent;
//#if !AOT
// AOT usage seems ok in this class
//using Fasterflect; 
using LionFire.Reflection.Fasterflect;
//#endif
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using LionFire.Applications;
using Microsoft.Extensions.Logging;
using LionFire.Vos;
using LionFire.Dependencies;
using LionFire.Structures;

namespace LionFire.Assets
{

    /// <summary>
    /// Currently only used during hardcoded creation of Packages at the beginning of an app load.
    /// Also see: PackageMounter
    /// </summary>
    public class PackageFactoryManager
    {
        private static ILogger l = Log.Get();
        private static ILogger lProfiling = Log.Get("Profiling." + typeof(PackageFactoryManager).FullName);

        #region Packages

        public static ConcurrentDictionary<string, Package> Packages => packages;
        private static ConcurrentDictionary<string, Package> packages = new ConcurrentDictionary<string, Package>();

        #endregion

        static private Dictionary<string, Type> packageTypes;

        public static void DetectPackageTypes()
        {
            var detectedTypes = new Dictionary<string, Type>();

            var sb = new StringBuilder();
            sb.AppendLine("== Detecting packages in current AppDomain: ==");

            int count = 0;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (
                    assembly.FullName.Contains("Bullet")
                    )
                    continue;

                foreach (Type type in
#if AOT
                        (IEnumerable)
#endif
                    assembly.TypesWith<PackageNameAttribute>())
                {
                    var attr = type.GetCustomAttribute<PackageNameAttribute>();
                    if (attr == null)
                    {
                        l.Error("Got type that should have PackageNameAttribute but doesn't: " + type.FullName);
                        continue;
                    }

                    count++;
                    sb.AppendLine(" - " + attr.Name);
                    detectedTypes.Add(attr.Name, type);
                }
            }
            sb.AppendLine("== Detected " + count + " packages ==");
            l.Info(sb.ToString());
            packageTypes = detectedTypes;
        }

        public static bool OverwriteDefault =
#if DEV
            false;
#else
            true;
#endif

        public static bool SaveToDiskDefault = true;

        public interface ISplashScreen
        {
            void SetMessage(string msg);
        }
        public static bool EnsurePackageAvailable(Type packageType, bool? overwrite = null, bool? saveToDisk = null)
        //where PackageType : IPackageFactory
        {
            DependencyContext.Current.GetService<ISplashScreen>()?.SetMessage("Loading package: " + packageType.Name.Replace("Pack", ""));
            try
            {
                //l.Trace("[package] Ensuring package available: " + packageType.Name);
                var sw = Stopwatch.StartNew();
                Type packageFactoryType = typeof(PackageFactory<>).MakeGenericType(packageType);

                //// FUTURE TODO: Try loading package from disk first ?
                var ensureCreatedMethodInfo = packageFactoryType.GetMethod("EnsurePackageCreated", BindingFlags.Static | BindingFlags.Public);

                bool ow = overwrite.HasValue ? overwrite.Value : OverwriteDefault;
                bool std = saveToDisk.HasValue ? saveToDisk.Value : SaveToDiskDefault;
                ensureCreatedMethodInfo.Invoke(null, new object[] { ow, std });

                lProfiling.Info("-EnsurePackageAvailable: " + packageType.Name + " done (" + sw.ElapsedMilliseconds + "ms)");
                return true;
            }
            finally
            {
                DependencyContext.Current.GetService<ISplashScreen>()?.SetMessage("Loading package: " + packageType.Name.Replace("Pack", "") + " done");
            }

        }

        public static bool EnsurePackageAvailable(string packageName)
        {
            bool allowRetry = true;
            retry:
            if (packageTypes == null)
            {
                DetectPackageTypes();
                allowRetry = false;
            }

            Type packageType =
#if AOT
				(Type)
#endif
                packageTypes.TryGetValue(packageName);

            if (packageName == null)
            {
                if (allowRetry)
                {
                    allowRetry = false;
                    goto retry;
                }
                else
                {
                    return false;
                }
            }

            //Type genericFactoryType = typeof(PackageFactory<>).MakeGenericType(packageType);

            //MethodInfo mi = EnsurePackageAvailableMethodInfo.MakeGenericMethod(genericFactoryType);
            //mi.Invoke(null, null);

            EnsurePackageAvailable(packageType);

            return true;
        }

        //static MethodInfo EnsurePackageAvailableMethodInfo
        //{
        //    get
        //    {
        //        if (ensurePackageAvailableMethodInfo == null)
        //        {
        //            ensurePackageAvailableMethodInfo = typeof(PackageFactoryManager).Method("EnsurePackageAvailableForType");
        //            if (ensurePackageAvailableMethodInfo == null)
        //            {
        //                throw new Exception("Failed to get MethodInfo for PackageFactoryManager.EnsurePackageAvailableForType");
        //            }
        //        }
        //        return ensurePackageAvailableMethodInfo;
        //    }

        //}
        //static MethodInfo ensurePackageAvailableMethodInfo;

        //static MethodInfo EnsureCreatedMethodInfo
        //{
        //    get
        //    {
        //        if (ensureCreatedMethodInfo == null)
        //        {
        //            ensureCreatedMethodInfo = typeof(PackageFactory<>).Method("EnsureCreated");
        //            if (ensureCreatedMethodInfo == null)
        //            {
        //                throw new Exception("Failed to get MethodInfo for PackageFactory<>.EnsureCreated");
        //            }
        //        }
        //        return ensureCreatedMethodInfo;
        //    }
        //}
        //static MethodInfo ensureCreatedMethodInfo;
    }

    public interface IPackageFactory
    {
        void EnsureCreated(bool overwrite = true, bool saveToDisk = true);
    }

    public abstract class PackageFactory<T> : IPackageFactory
        where T : class, IPackageFactory, new()
    {
        private static ILogger l = Log.Get();
        public const bool SaveToDiskDefault =
 //#if DEPLOY
 //            false;
 //#else
 true;
        //#endif

        #region (Static Public)

        public static void EnsurePackageCreated(bool overwrite = true, bool saveToDisk = SaveToDiskDefault)
        {
            Instance.EnsureCreated(overwrite, saveToDisk);
        }

        public static T Instance => ManualSingleton<T>.GuaranteedInstance;

        //public static PackageFactory<T> FactoryInstance { get { return Singleton<PackageFactory<T>>.Instance; } }

        #endregion

        #region (Public) Methods

        public void EnsureCreated(bool overwrite = true, bool saveToDisk = SaveToDiskDefault)
        {
            l.Trace(this.GetType().Name + ".EnsureCreated(" + overwrite + ", " + saveToDisk + ")");

            if (package == null)
            {
                if (!overwrite)
                {
                    int mountCount = VosApp.Instance.PackageMounter.TryEnsurePackageMounted(PackageName, false);
                    if (mountCount > 0)
                    {
                        l.Debug("EnsureCreated: package already found and overwrite package is disabled: " + PackageName);
                        return;
                    }
                }
                package = Create(saveToDisk, overwrite: overwrite);
            }
        }

        #endregion

        #region (Public) Properties

        #region Package

        public Package Package
        {
            get
            {
                EnsureCreated();
                return package;
            }
            private set { package = value; }
        }
        private Package package;

        #endregion

        public virtual string PackageName
        {
            get
            {
                if (packageName == null)
                {
                    var attrs = this.GetType().GetCustomAttributes(typeof(PackageNameAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        packageName = ((PackageNameAttribute)attrs[0]).Name;
                    }
                    else
                    {
                        throw new Exception("No package name set for this type: " + this.GetType().FullName + ".  Set it by overriding the PackageName property or applying PackageNameAttribute.");
                    }
                }
                return packageName;
            }
        }
        private string packageName;

        #endregion

        //public bool DisableWritingToDisk = false;
        public string DefaultFactoryStoreName =
#if DEPLOY
            VosStoreNames.VarBase; // TODO - somehow have a readonly set of files
#else
#if UNITY
			VosStoreNames.VarBase;
#else
			VosStoreNames.AppBase;
#endif
#endif

#if TRACE_CREATE
        private static int indent = 0;
#endif

        private Package Create(bool saveToDisk = true, string storeName = null, bool overwrite = false)
        {
            if (storeName == null)
                storeName = DefaultFactoryStoreName;

#if OLD
            var existing = new HAsset
#if !AOT
                <Assets.Package>(PackageName);
#else
			(obj: null, assetPath: PackageName, assetType: typeof(Package));
#endif
#else
#if !AOT
            var existing = PackageName.ToHAsset<Assets.Package>();
#else
            throw new NotImplementedException();
#endif
#endif

            if (existing.Value != null)
            {
                if (!overwrite)
                {
                    l.Warn(">>! Package already exists: " + PackageName + ". " + Environment.StackTrace);
                    return existing.Value;
                }
                else
                {
                    l.Debug(">>. Package already exists: " + PackageName + ".  Overwriting it.");
                }
            }

            using (new VosContext
            {
                DisableWritingToDisk = !saveToDisk,
                IgnoreReadonly = true,
                Package = PackageName,
                Store = storeName,
            })
            {

#if TRACE_CREATE
                var msg = new StringBuilder();
                msg.Append("> " + indent);
                for (int i = 0; i < indent*4; i++) msg.Append(" ");
                msg.Append(">>> Package: " + PackageName + ", Store: " + storeName);
                l.Info(msg.ToString());

                indent ++;
#endif

                foreach (string dependencyName in
#if AOT
 (IEnumerable)
#endif
                    PackageDependencies)
                {
                    try
                    {
                        bool dependencyOk = PackageFactoryManager.EnsurePackageAvailable(dependencyName);
                        if (!dependencyOk)
                        {
                            throw new Exception("Package named '" + dependencyName + "' not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        l.Error("Failed to get dependency package named '" + dependencyName + "'.  Exception: " + ex.ToString());
                        throw;
                    }
                }

                //var previousDefaultCreatePackage = AssetContext.Current.DefaultCreatePackage;
                //var previousDefaultSavePackage = AssetContext.Current.DefaultSavePackage;

                //using (new VosContext
                //{
                //    DisableWritingToDisk = !saveToDisk,
                //    IgnoreReadonly = true,
                //    Package = PackageName,
                //    Store = storeName,
                //})
                //{
                //if (useCreatePackage && previousDefaultCreatePackage != null)
                //{
                //    l.Debug(" >> previousDefaultCreatePackage: " + previousDefaultCreatePackage + " new DefaultCreatePackage: " + Package);
                //}
                //if (useSavePackage && previousDefaultSavePackage != null)
                //{
                //    if (useSavePackage) l.Debug(" >> previousDefaultCreatePackage: " + previousDefaultCreatePackage + " new DefaultCreatePackage: " + Package);
                //}
                HAsset<Package> hPackage;
                Package package;
                try
                {
                    l.Info("Creating package: '" + PackageName + "'");

                    VosApp.Instance.PackageMounter.MountPackageAtStore(PackageName, true);

                    hPackage = ConstructPackageObject(PackageName);
                    package = hPackage.Value;

                    //if (useCreatePackage) AssetContext.Current.DefaultCreatePackage = Package;
                    //if (useSavePackage) AssetContext.Current.DefaultSavePackage = Package;

                    //Package.Save(); IAsset.Save
                    hPackage.Save();

                    OnCreating();
                    return package;
                }
                catch (Exception ex)
                {
                    l.Error("Exception while creating package: " + ex.ToString());
                    return null;
                }
                finally
                {
#if TRACE_CREATE
                        indent--;
                        msg = new StringBuilder();
                        msg.Append("< " + indent);
                        for (int i = 0; i < indent * 4; i++) msg.Append(" ");
                        msg.Append("<<< Package: " + PackageName + ", Store: " + storeName);
                        l.Info(msg.ToString());
#endif

                    //if (useCreatePackage) l.Debug(" << reverting to previousDefaultCreatePackage: " + previousDefaultCreatePackage + " from DefaultCreatePackage: " + Package);
                    //if (useSavePackage) l.Debug(" << reverting to previousDefaultSavePackage: " + previousDefaultCreatePackage + " from DefaultCreatePackage: " + Package);

                    //if (useCreatePackage) AssetContext.Current.DefaultCreatePackage = previousDefaultCreatePackage;
                    //if (useSavePackage) AssetContext.Current.DefaultSavePackage = previousDefaultSavePackage;
                }
                //}


                //AssetContext.Current.Packages.Add(package);

            }
        }

        public const bool useSavePackage = true;
        //public const bool useCreatePackage = false;

        protected HAsset
            <Package>
                ConstructPackageObject(string name)
        {
            //Package = new Package(new AssetID(name + ":" + name))

            HAsset<Package> hAsset = name;
            if (hAsset.Value == null)
            {
                var package = new Package(name)
                {
                    EnabledByDefault = true, // TEMP - change to false
                };
                hAsset.Value = package;
            }
            return hAsset;
            
#if OLD
            //protected virtual VobHandle<Package> ConstructPackageObject(string name)

			VobHandle<Package> vh = name.ToAssetVobHandle<Package>();
            
			//VobHandle<Package> vh = new VobHandle<Package>(VosApp.Instance.Packages[name]);
			vh.Object = Package;

			return vh;
#else

#endif
        }

        /// <summary>
        /// FUTURE: Require specific versions of dependencies
        /// </summary>
        public virtual IEnumerable<string> PackageDependencies
        {
            get
            {
                yield break;
            }
        }

        public abstract void OnCreating();
    }
}

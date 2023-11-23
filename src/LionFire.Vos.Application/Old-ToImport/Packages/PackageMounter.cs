#if false //TODO // Merge functionality in
//#define ENABLE_APPDATA_FALLBACK
//#define NEVER NEVER!

//#define TRACE_PACK_QUERY_FAIL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.LionRing;
//using LionFire.Identity;
using LionFire.Serialization;
using System.Threading;
using System.IO;
using LionFire.ObjectBus;
using LionFire.Persistence.Filesystem;
using LionFire.Collections;


#if RAPTORDB
using LionFire.ObjectBus.RaptorKV;
#endif
using LionFire.Assets;
using System.Collections;
using LionFire.Vos;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;
using LionFire.Vos.Mounts;

namespace LionFire.Vos.Packages
{
    // TODO: Keep PackageMounts in a Vobo here:
    //[Ignore(LionSerializeContext.Persistence)]
    //public class PackageMounts : Vobo<PackageMounts>
    //{
    //    public List<Mount> Mounts
    //    {
    //        get { return mounts; }
    //        set { mounts = value; }
    //    }
    //    private List<Mount> mounts;
    //}

    public class PackageMounter
    {
        //public List<Mount> VosMounts
        //{
        //    get { return vosMounts; }
        //} private List<Mount> vosMounts = new List<Mount>();

        #region Packages

        #region PackageDirectories

        public List<PackageDirectory> PackageDirectories => packageDirectories; 
        private List<PackageDirectory> packageDirectories = new List<PackageDirectory>();

        private Dictionary<string, List<Mount>> mountsForPackages = new Dictionary<string, List<Mount>>(); // TODO - move 

        public IEnumerable<string> PackageNames
        {
            get
            {
                foreach (var packageDirectory in PackageDirectories.Select(pd => pd.Path))
                {
                    if (!Directory.Exists(packageDirectory)) continue;
                    foreach (var packageSubDirectory in Directory.GetDirectories(packageDirectory))
                    {
                        var packageDirName = Path.GetFileName(packageSubDirectory);
                        if (!packageDirName.StartsWith("[")) continue;
                        if (!packageDirName.EndsWith("]")) continue;
                        var name = packageDirName.TrimEnd(']').TrimStart('[');
                        yield return name;
                    }
                }
            }
        }

        public void MountAllPackages()
        {
            foreach (string name in
#if AOT
 (IEnumerable)
#endif
 PackageNames)
            {
                MountPackage(name);
            }
        }

        public void UnmountAllPackages()
        {
            throw new NotImplementedException();
        }

#if AOT
		public  List<Mount> GetOrAddDefault( Dictionary<string,List<Mount>> dictionary,
		                                    string key, Func<List<Mount>> defaultValue)
		{
			if (dictionary.ContainsKey(key)) return dictionary[key];
			List<Mount> val = defaultValue.Invoke();

			var keyed = val as IStringKeyed;
			if (keyed != null) { keyed.Key = key; }
			
			dictionary.Add(key, val);
			return val;
		}
#endif
        private List<Mount> GetActiveMountsForPackage(string packageName)
        {
            //Vob vob = VosApp.Instance.PackageMounts[packageName].AsType<object>();

            //VobHandle<PackageMounts> pm = new VobHandle<PackageMounts>(VosApp.Instance.PackageMounts[packageName]
            //PackageMounts packageMount = vob.AsType<PackageMounts>();


#if AOT
			var mounts = GetOrAddDefault(mountsForPackages, packageName, () => new List<Mount>());
#else
            var mounts = mountsForPackages.GetOrAddDefault(packageName, () => new List<Mount>());
#endif
            return mounts;

        }

        public int TryEnsurePackageMounted(string packageName, bool writable = false)
        {
            // REVIEW: What happens if 
            var mounts = GetActiveMountsForPackage(packageName).Where(m => m.Target.Scheme == FileReference.Constants.UriScheme);

            int count = writable ? mounts.Where(m => !m.Options.IsReadOnly()).Count() : mounts.Count();

            if (count == 0)
            {
                count = MountPackage(packageName, writable); // count here includes only physical mounts
            }

            return count;

        }

#if OLD // Now use PackageMounter, which uses /.../available to find packages, instead of filesystem directories, and thus avoids touching the filesystem directly

        public int MountPackage(string packageName, bool writable = false)
        {
            //List<Mount> mountsForPackage = mountsForPackages.TryGetValue(packageName);
            List<Mount> mountsForPackage = GetActiveMountsForPackage(packageName);

            //bool created = mountsForPackage == null;
            //if (mountsForPackage == null)
            //{
            //    mountsForPackage = new List<Mount>();
            //}

            if (mountsForPackage.Count > 0)
            {
                //l.Trace("MountPackage: already mounted " + mountsForPackage.Count + " mounts for package: " + packageName);// Normal
                return 0;
            }

            int mounted = 0;
            foreach (var packageDirectory in PackageDirectories)
            {
                if (packageDirectory.MountOptions.IsReadOnly && writable) continue;

                string path = Path.Combine(packageDirectory.Path, VosPath.PackageNameToStorageSubpath(packageName));

                if (Directory.Exists(path))
                {
                    var diskReference = path.ToFileReference();
                    var physicalLocation = VosApp.Instance.PackageStores[packageName, packageDirectory.LocationName];
                    var logicalLocation = VosApp.Instance.Packages[packageName];
                    var overlayLocation = VosApp.Instance.ActiveData;
                    // 3 mounts:
                    //  - ActiveData: /App/$/...
                    //  - Package: /App/_/Packages/PackageName/...
                    //  - PackageLayer: /App/_/PackageLayers/PackageName/LayerName/... (TODO)

                    // Archive
                    {
                        var vosLocation = physicalLocation;
                        var targetLocation = diskReference;

                        if (!HasMount(mountsForPackage, vosLocation, targetLocation))
                        {
                            // TODO: Use VobReference instead of FileReference here?
                            //Mount mount = new Mount(vosLocation, path.AsFileReference(), packageName, packageDirectory.LocationName, true, packageDirectory.MountOptions);
                            Mount mount = new Mount(vosLocation, targetLocation, packageName, null, true, packageDirectory.MountOptions);


                            //if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
                            //{
                            //    l.Info("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + mount.Vob.Path);
                            //    continue;
                            //}
                            //vosMounts.Add(mount);
                            mountsForPackage.Add(mount);
                            l.Info("[PACK archive] '" + packageName + "' pack archive mounted at " + vosLocation + " -> " + targetLocation);

                            mounted++;
                        }
                        else
                        {
                            l.TraceWarn("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + vosLocation.Path);
                        }
                    }

                    // Package
                    {
                        var vosLocation = logicalLocation;
                        var targetLocation = physicalLocation;

                        //if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
                        if (!HasMount(mountsForPackage, vosLocation, targetLocation.Reference))
                        //if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
                        {
                            Mount mount = new Mount(vosLocation, targetLocation.Reference, packageName, packageDirectory.LocationName, true, packageDirectory.MountOptions);

                            //if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
                            //{
                            //    l.TraceWarn("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + mount.Vob.Path);
                            //    continue;
                            //}
                            //vosMounts.Add(mount);
                            mountsForPackage.Add(mount);
                            l.Debug("[pack overlay] '" + packageName + "' pack mounted: " + vosLocation + " -> " + targetLocation);
                            //mounted++;
                        }
                        else
                        {
                            l.TraceWarn("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + vosLocation.Path);
                        }
                    }

                    // Overlay
                    {
                        var vosLocation = overlayLocation;
                        var targetLocation = VosApp.Instance.Packages[packageName];

                        // TODO: Use VobReference instead of FileReference here?
                        //Mount mount = new Mount(vosLocation, path.AsFileReference(), packageName, packageDirectory.LocationName, true, packageDirectory.MountOptions);

                        //if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
                        if (!HasMount(mountsForPackage, vosLocation, targetLocation.Reference))
                        {

                            Mount mount = new Mount(vosLocation, targetLocation, packageName, null, true, packageDirectory.MountOptions);

                            // TODO FIXME: Mounting/Unmounting overlay multiple times for multiple physical archives.


                            //vosMounts.Add(mount);
                            mountsForPackage.Add(mount);
                            l.Trace("[pack global overlay] '" + packageName + "' pack overlay enabled at " + vosLocation + " -> " + targetLocation);

                            //mounted++;
                        }
                        else
                        {
                            //l.Trace("Skipping, because mount path already found in mountsForPackage: " + vosLocation.Path + " --> " + targetLocation); // Normal
                        }
                    }


                }
#if TRACE_PACK_QUERY_FAIL
                else
                {
                    l.Trace("[PACK] '" + packageName + "' pack not found at " + path);
                }
#endif
            }

            if (mounted > 0)
            {
                //mountsForPackages.Add(packageName, mountsForPackage);
            }
            else
            {
                if (mounted == 0 && mountsForPackage.Count == 0)
                {
                    l.Warn("[PACK] '" + packageName + "' package mount did not find package in any known package directories.  (Mode: " + (writable ? "READABLE & WRITABLE" : "READABLE") + ")");
                }
            }
            return mounted;
        }
        private bool HasMount(List<Mount> mountsForPackage, Vob vob, IReference target)
        {
            return mountsForPackage.Where(m => m.Vob.Path == vob.Path && m.Target.Equals(target)).Any();
        }

        public bool UnmountPackage(string packageName)
        {
            var mounts =
#if AOT
				(List<Mount>)
#endif
 mountsForPackages.TryGetValue(packageName);

            if (mounts == null) return false;

            foreach (var mount in mounts)
            {
                mount.IsEnabled = false;
                //vosMounts.Remove(mount);
                l.Debug("Package '" + packageName + "'unmounted from: " + mount.Target.ToString());
            }

            mountsForPackages.Remove(packageName);
            return true;
        }
#endif

        #endregion

        private readonly object mountsLock = new object();

        //public IEnumerable<Mount> GetMountsForPackage(string name)
        //{
        //    IEnumerable<Mount> mountsForPackage;
        //    mountsForPackage = mountsForPackages.TryGetValue(name);
        //    if (mountsForPackage == null) return new Mount[] { };
        //    return mountsForPackage;
        //}

        private Mount TryMount(Vob mountPoint, Vob mountActual, MountOptions mountOptions, string packageName)
        {
            if (mountPoint == null) throw new ArgumentNullException("mountPoint");
            if (mountActual == null) throw new ArgumentNullException("mountActual");
            if (mountOptions == null)
            {
                // ENH: Inherit default mount options from mountActual, remove IsExclusive
                throw new ArgumentNullException("mountOptions");
            }

            if (mountPoint.Mounts.ContainsKey(mountActual.Key))
            {
                l.Debug("Already mounted: " + mountPoint + " ==> " + mountActual);
                return mountPoint.Mounts[mountActual.Key];
            }

            // TODO: Use VobReference instead of FileReference here?
            //Mount mount = new Mount(vosLocation, path.AsFileReference(), packageName, packageDirectory.LocationName, true, packageDirectory.MountOptions);

            Mount mount = new Mount(mountPoint, mountActual, packageName, null, true, mountOptions);

            //if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
            //{
            //    l.Info("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + mount.Vob.Path);
            //    continue;
            //}

            //vosMounts.Add(mount);
            //mountsForPackage.Add(mount);
            //l.Info("[PACK] '" + packageName + "' pack archive mounted at " + mountPoint + " -> " + mountActual);

            return mount;
        }

        public KeyValuePair<Vob, Mount> GetStore(string storeName = null)
        {
            if (storeName == null)
            {
                storeName = VosContext.Current.Store;
            }
            if (storeName == null)
            {
                throw new ArgumentException("storeName == null and VosContext.Current.Store == null also");
            }

            Vob storeVob = V.Stores.QueryChild(storeName);

            if (storeVob == null)
            {
                throw new VosException("Store '" + storeName + "' not mounted.  (FUTURE: Automount Stores that are configured for it)");
            }

            if (storeVob.Mounts.Count > 1)
            {
                throw new VosException("Physical mount location has more than one mount: " + storeVob);
            }

            var physicalMount = storeVob.Mounts.Values.SingleOrDefault();

            if (physicalMount == null)
            {
                throw new VosException("Store '" + storeName + "' not mounted (no mount).  (FUTURE: Automount Stores that are configured for it)");
            }

            if (!physicalMount.MountOptions.IsExclusive)
            {
                throw new VosException("Store '" + storeName + "' mount is not mounted as Exclusive");
            }
            return new KeyValuePair<Vob, Mount>(storeVob, physicalMount);
        }

        public Vob DefaultMountPoint
        {
            get
            {
                return defaultMountPoint ?? VosApp.Instance.ActiveData;
            }
            set
            {
                defaultMountPoint = value;
            }
        } private Vob defaultMountPoint;

        /// <summary>
        /// mountActual is V.Stores[storeName]["[packageName]"]
        /// </summary>
        /// <param name="mountPoint"></param>
        /// <param name="packageName"></param>
        /// <param name="mountIfMissing"></param>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public bool MountPackageAtStore(string packageName, bool mountIfMissing = true, Vob mountPoint = null, string storeName = null)
        {
            if (mountPoint == null) mountPoint = DefaultMountPoint;

            if (storeName == null) storeName = VosContext.Current.Store;

            var kvp = GetStore(storeName);

            Vob storeVob = kvp.Key;
            MountOptions mountOptions = new MountOptions(kvp.Value.MountOptions);
            mountOptions.IsExclusive = false;

            string packageStorageName = VosPath.PackageNameToStorageSubpath(packageName);

            Vob physicalLocation = storeVob.QueryChild(packageStorageName);

            if (physicalLocation == null)
            {
                if (!mountIfMissing)
                {
                    l.Debug("Store '" + storeName + "' does not contain package '" + packageName + "' at its root and mountIfMissing is false");
                    // Allow Store subpath?  Then you might as well just provide a VobReference for mountActual.
                    return false;
                }
                else
                {
                    physicalLocation = storeVob[packageStorageName];
                }
            }

            //PackageDirectory packageDirectory;

            var archiveLocation = V.Archives[packageName][storeName]; // Exclusive

            var packageOverlay = VosApp.Instance.Packages[packageName]; // Overlay
            var mainOverlay = VosApp.Instance.ActiveData; // Overlay

            List<Mount> mountsForPackage = GetActiveMountsForPackage(packageName);

            Mount packageMount;
            packageMount = TryMount(archiveLocation, physicalLocation, mountOptions, packageName);
            //if(packageMount==null
            packageMount = TryMount(packageOverlay, physicalLocation, mountOptions, packageName);
            packageMount = TryMount(mainOverlay, packageOverlay, mountOptions, packageName);

            return true;
            // 3 mounts:
            //  - ActiveData: /App/$/...
            //  - Package: /App/_/Packages/PackageName/...
            //  - PackageLayer: /App/_/PackageLayers/PackageName/LayerName/... (TODO)

            // Archives[packageName][locationName]
            {


                //var mountPoint = archiveLocation;
                //var mountActual = physicalLocation;

                //// TODO: Use VobReference instead of FileReference here?
                ////Mount mount = new Mount(vosLocation, path.AsFileReference(), packageName, packageDirectory.LocationName, true, packageDirectory.MountOptions);
                //Mount mount = new Mount(mountPoint, mountActual, packageName, null, true, packageDirectory.MountOptions);

                //if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
                //{
                //    l.Info("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + mount.Vob.Path);
                //    continue;
                //}

                //vosMounts.Add(mount);
                //mountsForPackage.Add(mount);
                //l.Info("[PACK] '" + packageName + "' pack archive mounted at " + mountPoint + " -> " + mountActual);

                //mounted++;
            }

            // Package[packageName]
            //            {
            //                var mountPoint = packageOverlay;
            //                var mountActual = physicalLocation;
            //                Mount mount = new Mount(mountPoint, mountActual, packageName, packageDirectory.LocationName, true, packageDirectory.MountOptions);

            //                if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
            //                {
            //                    l.Info("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + mount.Vob.Path);
            //                    continue;
            //                }
            //                vosMounts.Add(mount);
            //                mountsForPackage.Add(mount);
            //                l.Debug("[pack] '" + packageName + "' pack mounted: " + mountPoint + " -> " + mountActual);
            //                //mounted++;
            //            }

            //#warning FIXME - Move this to separate method. Only mount if package not mounted yet.
            //            //MountPackage(packageName, true);

            //            // Overlay V.ActiveData[...]
            //            {
            //                var mountPoint = mainOverlay;
            //                var mountActual = VosApp.Instance.Packages[packageName];

            //                if (mountPoint.Mounts.ContainsKey(mountActual.Reference.ToString()))
            //                {

            //                }
            //                else
            //                {

            //                    // TODO: Use VobReference instead of FileReference here?
            //                    //Mount mount = new Mount(vosLocation, path.AsFileReference(), packageName, packageDirectory.LocationName, true, packageDirectory.MountOptions);
            //                    Mount mount = new Mount(mountPoint, mountActual, packageName, null, true, packageDirectory.MountOptions);

            //                    // TODO FIXME: Mounting/Unmounting overlay multiple times for multiple physical archives.

            //                    if (mountsForPackage.Where(m => m.Vob.Path == mount.Vob.Path).Any())
            //                    {
            //                        l.Info("UNTESTED: Skipping, because mount path already found in mountsForPackage: " + mount.Vob.Path);
            //                        continue;
            //                    }
            //                    vosMounts.Add(mount);
            //                    mountsForPackage.Add(mount);
            //                    l.Debug("[pack] '" + packageName + "' pack overlay enabled at " + mountPoint + " -> " + mountActual);

            //                    //mounted++;
            //                }
            //            }

        }

        public void EnsurePackageCreatedAndMounted(string packageName, bool writable = false)
        {
            //VobHandle<Package> package = new VobHandle<Package>();

            lock (mountsLock)
            {
                List<Mount> mountsForPackage = GetActiveMountsForPackage(packageName);

                //mountsForPackage = mountsForPackages.TryGetValue(packageName);
                if (mountsForPackage.Where(m => writable ? !m.MountOptions.IsReadOnly : true).Any()) return;

                // Try mounting
                l.Trace("[pack] Attempting to mount package " + packageName + " (" + (writable ? "RW" : "RO") + ")");
                MountPackage(packageName, writable); // If writable == true, mount first writable location

                if (mountsForPackage.Where(m => writable ? !m.MountOptions.IsReadOnly : true).Any()) return;

                //            mountsForPackage = mountsForPackages.TryGetValue(packageName);
                //            if (mountsForPackage != null
                //&& (!writable || mountsForPackage.Where(m => !m.MountOptions.IsReadOnly).Any())) return;

                // Mounting failed, so try creating

                // CreatePackageDirectory MUST BE writable or this will fail if writable == true!

                //string path = Path.Combine(CreatePackageDirectory.Path, VosPath.PackageNameToStorageSubpath(packageName));

                //Directory.CreateDirectory(path);

                string storeName = VosContext.Current.Store;

                MountPackageAtStore(packageName, true, mountPoint: V.ActiveData, storeName: storeName);

                //Vob storeVob = VosApp.Instance.Stores[storeName];
                //if (storeVob.Mounts == null)
                //{
                //    throw new VosException("Cannot create package in store " + storeName + " because that store is not mounted.");
                //}

                string path = storeName; // TEMP
                l.Trace("[pack] Attempting to create a mount location for package " + packageName + " (" + (writable ? "RW" : "RO") + ") at store " + storeName);

                // TODO Set permissions?

                MountPackage(packageName, writable);

                //mountsForPackage = mountsForPackages.TryGetValue(packageName);

                if (mountsForPackage.Where(m => writable ? !m.MountOptions.IsReadOnly : true).Any())
                {
                    l.Info("[PACK] '" + packageName + "' pack created (and mounted) at " + path);
                    return;
                }
                else
                {
                    throw new VosException("Failed to create and mount " + (writable ? "READABLE & WRITABLE" : "READABLE") + "package at path:" + path);
                }

                // OLD
                //if (mountsForPackage == null || mountsForPackage.TryGetValue(packageName) == null)
                //{
                //    throw new VosException("Failed to create and mount " + (writable ? "READABLE & WRITABLE" : "READABLE") + "package at path:" + path);
                //}
                //else
                //{
                //    l.Info("[PACK] '" + packageName + "' pack created (and mounted) at " + path);
                //}
            }
        }


        //public void InitVos()
        //{
        //    lock (mountsLock)
        //    {
        //        MountAllPackages();
        //    }
        //}

        //MountOptions PackMountOptions;

        #endregion

        #region Misc

        private static readonly ILogger l = Log.Get();
        
        #endregion


    }
}
#endif
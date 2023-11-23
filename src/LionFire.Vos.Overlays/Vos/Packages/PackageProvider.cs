using LionFire.Ontology;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Vos.Internals;
using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Vos.Packages
{
    //public interface ICooperativeInitializableBase
    //{
    //    IEnumerable<string> Provides { get; }
    //    IEnumerable<string> Requires { get; }

    //}

    //public interface ICooperativeInitializable
    //{
    //    void Initialize();
    //}
    //public interface ICooperativeInitializableAsync
    //{
    //    Task Initialize();
    //}

    public class PackageProvider : IParentable<IVob>
    //, ICooperativeInitializable
    {
        #region Relationships

        public IVob Vob { get; private set; }
        //IVob IParentable<IVob>.Parent
        public IVob Parent
        {
            get => Vob;
            set // REVIEW - this should typically be set via constructor injection.
            {
                if (value == Vob) return;
                if (Vob != null) throw new NotSupportedException("Parent Vob already set to a different value.");
                Vob = value;
            }
        }

        // REVIEW - AvailableRoot should be Vob, since it will be fastest once things are cached there?
        public IVob AvailableRoot { get; }

        public IVob ActivatedRoot { get; }
        public IVob CombinedRoot { get; }

        #endregion

        #region Parameters

        public PackageProviderDefaults Defaults { get; }
        public PackageProviderOptions Options { get; }

        #endregion

        #region Construction

        //public PackageProvider(IVob vob) : this(vob, null, null) { }
        //public PackageProvider(IVob vob, PackageProviderDefaults packageProviderDefaults) : this(vob, packageProviderDefaults, null) { }
        public PackageProvider(IVob vob, PackageProviderDefaults packageProviderDefaults = null, PackageProviderOptions options = null)
        {
            Vob = vob;
            Defaults = packageProviderDefaults ?? PackageProviderDefaults.Default;
            Options = options ?? PackageProviderOptions.Default;

            AvailableRoot = Vob[Options.AvailableSubPath];
            ActivatedRoot = Vob.GetRelativeOrAbsolutePath(Options.ActivatedPath);
            CombinedRoot = Vob.GetRelativeOrAbsolutePath(Options.CombinedPath);
            CombinedRoot.GetOrAddOwn<VobMounts>().CanHaveMultiReadMounts = true;
            CombinedRoot.GetOrAddOwn<VobMounts>().CanHaveMultiWriteMounts = true;
        }

        private async Task Initialize()
        {
            if (IsAutoRegisterAvailablePackagesEnabled == true) {
                await this.TryAutoRegisterPackages().ConfigureAwait(false);
                //services.AddExistingPackageSources(name, packageProviderOptions: options); 
            }
        }
        #endregion

        #region Derived

        public bool IsAutoRegisterAvailablePackagesEnabled // FUTURE: Use overlay stack
            => Options.IsAutoRegisterAvailablePackagesEnabled ?? Defaults.IsAutoRegisterAvailablePackagesEnabled;

        #endregion

        #region State

        private readonly HashSet<string> enabledPackages = new HashSet<string>();
        private object _lock = new object();

        #endregion

        #region (Public) Properties
        //public IListHandle<Package> AvailablePackages { get; }

        public IEnumerable<string> AvailablePackages
        {
            get
            {
                // SetChildrenCollectionType<Package>("/data/*")
                // /data/main/..json
                // /data/mod1/..json
                return AvailableRoot.ReferenceGetListingsHandle<object>().Value.Value.Select(l=>l.Name);  // TODO - change list type to Package
                //return AvailableRoot.GetListHandle<Package>();  
            }
        }

        public IEnumerable<string> EnabledPackages => enabledPackages;
        public IEnumerable<string> DisabledPackages => AvailablePackages.Except(EnabledPackages);

        #endregion

        #region (Public) Methods

        public bool Enable(string packageName, int? readPriority = null, int? writePriority = null)  // ENH: consider change of parameters to have a partial overlay of MountOptions?
        {
            lock (_lock)
            {
                {
                    if (enabledPackages.Contains(packageName)) return false;
                    enabledPackages.Add(packageName);
                    var reference = Vob.Reference.GetChildSubpath(Options.AvailableSubPath, packageName);

                    var targetVob = reference.GetVob();

                    // ENH: Formalize overlay logic here?
                    //var effectiveReadPriority = OverlayValues.Get(AvailableRoot, packageName);
                    // effectiveReadPriority.Source is OverlayLayer
                    // effectiveReadPriority.Value is MountOptions
                    // OverlayValues.For(AvailableRoot).Layers = ...

                    var mount = AvailableRoot.QueryChild(packageName)?.AcquireOwn<VobMountOptions>();

                    if (readPriority == null) readPriority = mount?.ReadPriority ?? Options?.DefaultMountOptions?.ReadPriority;
                    if (writePriority == null) writePriority = mount?.WritePriority ?? Options?.DefaultMountOptions?.WritePriority;

                    if (readPriority == null && writePriority == null) throw new ArgumentException("Read and write priority could not be found.  Tried: 1) arguments to Enable, 2) AvailableRoot[packageName].AcquireOwn<VobMountOptions>(), and VosPackageManager.Options.DefaultMountOptions.");

                    CombinedRoot.Mount(reference, new VobMountOptions { ReadPriority = readPriority, WritePriority = writePriority, IsExclusive = false });
                }
                {
                    var reference = Vob.Reference.GetChildSubpath(Options.AvailableSubPath, packageName);
                    var vobMounts = reference.GetVob().AcquireOwn<VobMounts>();
                    if (vobMounts != null)
                    {
                        foreach (var mount in vobMounts.AllMounts.Where(m => m.Options?.MustBeManuallyEnabled == true))
                        {
                            mount.IsEnabled = true;
                        }
                    }
#if DEBUG 
                    else
                    {
                        Debug.WriteLine("Missing VobMounts on package in available folder.");  // TOLOG - sanity check fail
                    }
#endif
                }
                return true;
            }
        }

        public bool Disable(string packageName)
        {
            lock (_lock)
            {
                if (enabledPackages.Remove(packageName))
                {
                    var reference = Vob.Reference.GetChildSubpath(Options.AvailableSubPath, packageName);
                    var unmountedSomething = CombinedRoot.Unmount(reference) > 0;

                    if (unmountedSomething)
                    {
                        var vobMounts = reference.GetVob().AcquireOwn<VobMounts>();
                        if (vobMounts != null)
                        {
                            foreach (var mount in vobMounts.AllMounts.Where(m => m.Options?.MustBeManuallyEnabled == true))
                            {
                                mount.IsEnabled = false;
                            }
                        }
                    }
                    return unmountedSomething;
                }
            }
            return false;
        }

        #endregion
    }
}


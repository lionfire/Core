using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Vos.Internals;
using LionFire.Vos.Mounts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LionFire.Vos.Overlays
{
    public class OverlayStack : IParented<IVob>
    {
        private OverlayStackOptions Options { get; }
        public IVob Vob { get; private set; }
        IVob IParented<IVob>.Parent
        {
            get => Vob;
            set // REVIEW - this should typically be set via constructor injection.
            {
                if (value == Vob) return;
                if (Vob != null) throw new NotSupportedException("Parent Vob already set to a different value.");
                Vob = value;
            }
        }

        // TOTHINK - AvailableRoot should be Vob, since it will be fastest once things are cached there, right?
        public IVob AvailableRoot { get; }
        public IVob DataRoot { get; }

        #region Construction

        public OverlayStack(IVob vob) : this(vob, null) { }
        public OverlayStack(IVob vob, OverlayStackOptions options)
        {
            Vob = vob;
            Options = options ?? OverlayStackOptions.Default;
            AvailableRoot = Vob[Options.AvailableSubPath];
            DataRoot = Vob.GetRelativeOrAbsolutePath(Options.DataLocation);
            DataRoot.GetOrAddOwn<VobMounts>().CanHaveMultiReadMounts = true;
            DataRoot.GetOrAddOwn<VobMounts>().CanHaveMultiWriteMounts = true;
        }

        #endregion

        private readonly HashSet<string> enabledPackages = new HashSet<string>();
        private object _lock = new object();

        #region (Public) Methods

        public bool Enable(string packageName, int? readPriority = null, int? writePriority = null)  // ENH: consider change of parameters to have a partial overlay of MountOptions?
        {
            lock (_lock)
            {
                {
                    if (enabledPackages.Contains(packageName)) return false;
                    enabledPackages.Add(packageName);
                    var reference = Vob.Reference.GetChildSubpath(Options.AvailableSubPath, packageName);

                    var targetVob = reference.ToVob();

                    // ENH: Formalize overlay logic here?
                    //var effectiveReadPriority = OverlayValues.Get(AvailableRoot, packageName);
                    // effectiveReadPriority.Source is OverlayLayer
                    // effectiveReadPriority.Value is MountOptions
                    // OverlayValues.For(AvailableRoot).Layers = ...

                    var mount = AvailableRoot.QueryChild(packageName)?.GetOwn<MountOptions>();

                    if (readPriority == null) readPriority = mount?.ReadPriority ?? Options?.DefaultMountOptions?.ReadPriority;
                    if (writePriority == null) writePriority = mount?.WritePriority ?? Options?.DefaultMountOptions?.WritePriority;

                    if (readPriority == null && writePriority == null) throw new ArgumentException("Read and write priority could not be found.  Tried: 1) arguments to Enable, 2) AvailableRoot[packageName].GetOwn<MountOptions>(), and VosPackageManager.Options.DefaultMountOptions.");

                    DataRoot.Mount(reference, new MountOptions { ReadPriority = readPriority, WritePriority = writePriority, IsExclusive = false });
                }
                {
                    var reference = Vob.Reference.GetChildSubpath(Options.AvailableSubPath, packageName);
                    var vobMounts = reference.ToVob().GetOwn<VobMounts>();
                    if (vobMounts != null)
                    {
                        foreach (var mount in vobMounts.AllMounts.Where(m => m.Options?.IsManuallyEnabled == true))
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
                    var unmountedSomething = DataRoot.Unmount(reference) > 0;

                    if (unmountedSomething)
                    {
                        var vobMounts = reference.ToVob().GetOwn<VobMounts>();
                        foreach (var mount in vobMounts.AllMounts.Where(m => m.Options?.IsManuallyEnabled == true))
                        {
                            mount.IsEnabled = false;
                        }
                    }
                    return unmountedSomething;
                }
            }
            return false;
        }

        #endregion

        #region (Public) Properties

        public IEnumerable<string> AvailablePackages => throw new NotImplementedException();
        public IEnumerable<string> EnabledPackages => enabledPackages;
        public IEnumerable<string> DisabledPackages => AvailablePackages.Except(EnabledPackages);

        #endregion

    }
}


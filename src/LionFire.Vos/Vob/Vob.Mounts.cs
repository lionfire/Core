using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public partial class Vob
    {

        #region Mounts

        #region Mounts Collection

        #region HasMounts

        public bool HasMounts
        {
            get => hasMounts;
            private set
            {
                if (hasMounts == value)
                {
                    return;
                }

                hasMounts = value;
                if (hasMounts)
                {
                    vos.VobsWithMounts.Add(this);
                }
                else
                {
                    vos.VobsWithMounts.Remove(this);
                }
                InitializeEffectiveMounts();
            }
        }
        private bool hasMounts;

        private void UpdateHasMounts() => HasMounts = mounts != null && mounts.Count > 0;

        #endregion

        public MultiBindableDictionary<string, Mount> Mounts
        {
            get
            {

                if (mounts == null)
                {
                    mounts = new MultiBindableDictionary<string, Mount>();
                    mounts.CollectionChanged += new NotifyCollectionChangedHandler<Mount>(OnMountsCollectionChanged);
                    // MEMOPTIMIZE: Attach events.  Dispose dictionary when all are unmounted
                }
                return mounts;
            }
        }
        private MultiBindableDictionary<string, Mount> mounts;

        private readonly object mountsLock = new object();

        #endregion

        #region (internal) Mount method

        /// <summary>
        /// To mount create a new instance of Mount and set IsEnabled to true.
        /// </summary>
        /// <param name="mount"></param>
        internal void Mount(Mount mount)
        {
            {
                var ev = Mounting;
                if (ev != null)
                {
                    var args = new CancelableEventArgs<Mount>(mount);
                    ev(this, args);
                    if (args.IsCanceled)
                    {
                        return;
                    }
                }
            }

            lock (mountsLock)
            {
                // TODO EVENTS: mounting/mounted
                try
                {
                    Mounts.Add(mount);
                }
                catch (Exception ex)
                {
                    l.Info("Failed to mount with key " + Mounts.GetKey(mount) + " for " + mount + " " + ex);
                }
            }
        }

        #endregion

        #region Unmount methods

        private void _unmount(string mountKey, Mount mount)
        {
            if (mountKey != mount.Root.Key)
            {
                throw new ArgumentException("mountName mismatch for mount");
            }

            {
                var ev = Unmounting;
                if (ev != null)
                {
                    var args = new CancelableEventArgs<Mount>(mount);
                    ev(this, args);
                    if (args.IsCanceled)
                    {
                        return;
                    }
                }
            }

            lock (mountsLock)
            {
                // Fires mounts changed event. 
                // REVIEW: Fire events outside the lock?  Fire unmounting/mounting event outside the lock?
                Mounts.Remove(mountKey);
            }
        }

        internal void Unmount(string mountKey)
        {
            Mount knownMount = Mounts.TryGetValue(mountKey);
            if (knownMount == null)
            {
                // Already unmounted, if it ever was
                return;
            }
            _unmount(mountKey, knownMount);
        }

        internal void Unmount(Mount mount)
        {
            Mount knownMount = Mounts.TryGetValue(mount.Root.Key);
            if (!System.Object.ReferenceEquals(knownMount, mount))
            {
                return;
            }

            _unmount(mount.Root.Key, knownMount);
        }

        public void UnmountAll()
        {
            foreach (var mount in Mounts.Values.ToArray())
            {
                mount.IsEnabled = false;
            }
        }

        #endregion

        #region Mount Events

        public event Action<Vob, CancelableEventArgs<Mount>> Mounting;
        public event Action<Vob, CancelableEventArgs<Mount>> Unmounting;
        public event Action<Vob, Mount> Mounted;
        public event Action<Vob, Mount> Unmounted;

        private void OnMountsCollectionChanged(INotifyCollectionChangedEventArgs<Mount> e)
        {
            InitializeEffectiveMounts(); // OPTIMIZE TEMP - overkill

            UpdateHasMounts();
            vos.OnMountsChangedFor(this, e);

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                l.Warn("[MOUNT] Mounts collection reset (TODO: Handle)");
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        //                        l.Info("[MOUNT] " + this + " ==> " + item.RootHandle);
                        var ev = Mounted;
                        if (ev != null)
                        {
                            ev(this, item);
                        }
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        l.Info("[UNMOUNT] " + this + " ==> " + item.Root);
                        var ev = Unmounted;
                        if (ev != null)
                        {
                            ev(this, item);
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region EffectiveMounts

        internal readonly int VobDepth; // REVIEW - needed only for mounts?

        internal MultiValueSortedList<int, Mount> effectiveMountsByReadPriority;
        internal MultiValueSortedList<int, Mount> effectiveMountsByWritePriority;
        private Dictionary<string, Mount> effectiveMountsByName;
        private bool AreEffectiveMountsInitialized => effectiveMountsByReadPriority != null;
        //private Vob FirstAncestorWithMounts;
        //private int FirstAncestorWithMountsRelativeDepth;
        //private IEnumerable<string> FirstAncestorToThisSubPath;

        private bool InitializeEffectiveMounts(bool reset = false)
        {
            if (AreEffectiveMountsInitialized && !reset)
            {
                return true;
            }

            //// TODO: OPTIMIZE: To save memory, pass the buck to the first ancestor that has a mount.
            //if (!HasMounts)
            //{
            ////    effectiveMountsByReadPriority = null;
            ////    effectiveMountsByWritePriority = null;
            ////    effectiveMountsByName = null;

            ////    Vob firstAncestorWithMounts = null;
            ////    IEnumerable<string> firstAncestorToThisSubPath = null;
            ////    for (Vob ancestor = this.Parent; ancestor != null; ancestor = ancestor.Parent)
            ////    {
            ////        if (ancestor.HasMounts)
            ////        {
            ////            firstAncestorWithMounts = ancestor;
            ////            firstAncestorToThisSubPath = this.PathElements.Skip(ancestor.VobDepth);
            ////            break;
            ////        }
            ////    }
            ////    this.FirstAncestorWithMounts = firstAncestorWithMounts;
            ////    this.FirstAncestorToThisSubPath = firstAncestorToThisSubPath;
            ////    return;
            //}
            ////else
            ////{
            ////    this.FirstAncestorWithMounts = this;
            ////    this.FirstAncestorToThisSubPath = new string[] { };
            ////}

            effectiveMountsByReadPriority = new MultiValueSortedList<int, Mount>(
#if AOT
				Singleton<IntComparer>.Instance
#endif
);
            effectiveMountsByWritePriority = new MultiValueSortedList<int, Mount>(
#if AOT
				Singleton<IntComparer>.Instance
#endif
);
            effectiveMountsByName = new Dictionary<string, Mount>(); // FUTURE: If this is rarely used, create on demand

            bool gotSomething = false;
            for (Vob vob = this; vob != null; vob = vob.Parent)
            {
                foreach (KeyValuePair<string, Mount> kvp in
#if AOT
                        (IEnumerable)
#endif
 vob.Mounts)
                {
                    Mount mount = kvp.Value;
                    effectiveMountsByReadPriority.Add(mount.MountOptions.ReadPriority, mount);
                    gotSomething = true;

                    if (VosConfiguration.AllowIsReadonlyOverride || !mount.MountOptions.IsReadOnly)
                    {
                        effectiveMountsByWritePriority.Add(mount.MountOptions.WritePriority, mount);
                    }
                    effectiveMountsByName.Add(mount.Root.Key, mount);
                }
            }
            if (!gotSomething)
            {
                effectiveMountsByWritePriority = null;
                effectiveMountsByReadPriority = null;
                effectiveMountsByName = null;
                //l.Trace("Got no mounts for " + this.ToString());
                return false;
            }
            return true;
        }


        public void OnAncestorMountsChanged(Vob ancestor, INotifyCollectionChangedEventArgs<Mount> e)
        {
            // TODO: Adapt changes
            InitializeEffectiveMounts(reset: true);
        }

        #endregion

        #region Mounts

        /// <summary>
        /// Get the handle from the underlying mount (specified) representing this Vob's path and the specified type
        /// TOTEST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mount"></param>
        /// <returns></returns>
        private W<T> GetHandleFromMount<T>(Mount mount)
        {
            W<T> result;

            // TODO TO_ASSERT mount path is a parent of this.Path

            //int mountDepthDelta = this.VobDepth - mount.VobDepth; // MICROOPTIMIZE - move to mount

#if true
            if (mount.RootHandle is IProvidesHandleFromSubPath hp)
            {
                // (All that's needed here is IProvidesHandleOfDifferentType, but there is no interface for that yet.)

                // Some Handle types will be able to be smarter about providing handles....
                var parameter = Object.ReferenceEquals(mount.Vob, this) ? Enumerable.Empty<string>() : PathElements.Skip(mount.VobDepth);
                result = hp.GetHandleFromSubPath<T>(parameter);
            }
            else
            {
                var reference = Object.ReferenceEquals(mount.Vob, this) ? mount.RootHandle.Reference : mount.RootHandle.Reference.GetChildSubpath(PathElements.Skip(mount.VobDepth));
                // ... otherwise, we have to get the handle from the reference.
                result = reference.ToHandle<T>();
            }
#else
            if (Object.ReferenceEquals(mount.Vob, this))
            {
                if(mount.RootHandle is IProvidesHandleFromSubPath hp)
                {
                    // (All that's needed here is IProvidesHandleOfDifferentType, but there is no interface for that yet.)

                    // Some Handle types will be able to be smarter about providing handles....
                    result = hp.GetHandleFromSubPath<T>();
                }
                else
                {
                    // ... otherwise, we have to get the handle from the reference.
                    result = mount.RootHandle.Reference.GetHandle<T>();
                }
            }
            else
            {
                if (mount.RootHandle is IProvidesHandleFromSubPath hp)
                {
                    // (All that's needed here is IProvidesHandleOfDifferentType, but there is no interface for that yet.)

                    // Some Handle types will be able to be smarter about providing handles....
                    result = hp.GetHandleFromSubPath<T>(PathElements.Skip(mount.VobDepth));
                }
                else
                {
                    // ... otherwise, we have to get the handle from the reference.
                    result = mount.RootHandle.Reference.GetChildSubpath(PathElements.Skip(mount.VobDepth)).GetHandle<T>();
                }

                //result = mount.RootHandle[this.PathElements.Skip(mount.VobDepth)].ToHandle();
                //.GetHandle<T>(); // OPTIMIZE: cache this enumerable alongside the mount
            }
#endif

            //result.Mount = mount;
            return result;
        }


        /// <summary>
        /// The MountHandleObject itself is not significant -- it is just representing a handle to the root referene of the target Mount point
        /// </summary>
        /// <param name="mount"></param>
        /// <returns></returns>
        private RH<MountHandleObject> GetMountHandle(Mount mount)
        {
            RH<MountHandleObject> result;

            // TODO TO_ASSERT mount path is a parent of this.Path

            //int mountDepthDelta = this.VobDepth - mount.VobDepth; // MICROOPTIMIZE - move to mount

            //if (mountDepthDelta == 0)

            if (System.Object.ReferenceEquals(mount.Vob, this))
            {
                result = mount.RootHandle;
            }
            else
            {
                result = mount.RootHandle.Reference.GetChildSubpath(PathElements.Skip(mount.VobDepth)).ToReadHandle<MountHandleObject>(); // OPTIMIZE: cache this enumerable alongside the mount
            }

            //result.Mount = mount;
            return result;
        }

        internal string GetMountPath(Mount mount)
        {
            string result;

            if (System.Object.ReferenceEquals(mount.Vob, this))
            {
                result = mount.Root.Path;
                l.Trace("UNTESTED: (alt) MountPath: " + result);
            }
            else
            {
                result = LionPath.Combine(mount.Root.Path, PathElements.Skip(mount.VobDepth)); // OPTIMIZE: cache this enumerable alongside the mount
                l.Trace("UNTESTED: MountPath: " + result);
            }

            return result;
        }

        #endregion

    }
}

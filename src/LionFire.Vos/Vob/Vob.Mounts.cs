using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Referencing;

namespace LionFire.Vos
{
    public partial class Vob
    {
        #region Mounts

        #region Mounts Collection

        public MultiBindableDictionary<string, Mount> Mounts
        {
            get
            {

                if (mounts == null)
                {
                    mounts = new MultiBindableDictionary<string, Mount>();
                    //mounts.CollectionChanged += new NotifyCollectionChangedHandler<Mount>(OnMountsCollectionChanged);
                    // MEMOPTIMIZE: Attach events.  Dispose dictionary when all are unmounted
                }
                return mounts;
            }
        }
        private MultiBindableDictionary<string, Mount> mounts;

        private readonly object mountsLock = new object();

        #endregion

        #region (internal) Mount method

        internal void Mount(TMount tMount) => Mount(new Mount(this.Root, tMount));

        /// <summary>
        /// To mount create a new instance of Mount and set IsEnabled to true.
        /// </summary>
        /// <param name="mount"></param>
        internal void Mount(Mount mount)
        {
            GetVobNode().Mount(mount);
            // OLD TOTRIAGE
            //{
            //    var ev = Mounting;
            //    if (ev != null)
            //    {
            //        var args = new CancelableEventArgs<Mount>(mount);
            //        ev(this, args);
            //        if (args.IsCanceled)
            //        {
            //            return;
            //        }
            //    }
            //}

            //lock (mountsLock)
            //{
            //    // TODO EVENTS: mounting/mounted
            //    try
            //    {
            //        Mounts.Add(mount);
            //    }
            //    catch (Exception ex)
            //    {
            //        l.Info("Failed to mount with key " + Mounts.GetKey(mount) + " for " + mount + " " + ex);
            //    }
            //}
        }

        #endregion

        #region Unmount methods

        private void _unmount(string mountKey, Mount mount)
        {
            if (mountKey != mount.Target.Key)
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
            Mount knownMount = Mounts.TryGetValue(mount.Target.Key);
            if (!System.Object.ReferenceEquals(knownMount, mount))
            {
                return;
            }

            _unmount(mount.Target.Key, knownMount);
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

        //private void OnMountsCollectionChanged(INotifyCollectionChangedEventArgs<Mount> e)
        //{
        //    NextVobNode.InitializeEffectiveMounts(); // OPTIMIZE TEMP - overkill

        //    UpdateHasMounts();
        //    NextVobNode.OnMountsChangedFor(this, e);

        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
        //    {
        //        l.Warn("[MOUNT] Mounts collection reset (TODO: Handle)");
        //    }
        //    else
        //    {
        //        if (e.NewItems != null)
        //        {
        //            foreach (var item in e.NewItems)
        //            {
        //                //                        l.Info("[MOUNT] " + this + " ==> " + item.RootHandle);
        //                var ev = Mounted;
        //                if (ev != null)
        //                {
        //                    ev(this, item);
        //                }
        //            }
        //        }
        //        if (e.OldItems != null)
        //        {
        //            foreach (var item in e.OldItems)
        //            {
        //                l.Info("[UNMOUNT] " + this + " ==> " + item.Target);
        //                var ev = Unmounted;
        //                if (ev != null)
        //                {
        //                    ev(this, item);
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

        #endregion

        #region Mounts

        private THandle _GetHandleFromMount<T, TSubpathHandleProvider, THandle>(Mount mount, Func<TSubpathHandleProvider, IEnumerable<string>, THandle> subpathHandleProviderAction, Func<IReference, THandle> referenceToHandleAction)
        {
            THandle result;

            // TODO TO_ASSERT mount path is a parent of this.Path

            //int mountDepthDelta = this.VobDepth - mount.VobDepth; // MICROOPTIMIZE - move to mount


            if (mount.RootHandle is TSubpathHandleProvider subpathHandleProvider)
            {
                // (All that's needed here is IProvidesHandleOfDifferentType, but there is no interface for that yet.)

                // Some Handle types will be able to be smarter about providing handles....
                var parameter = ReferenceEquals(mount.Vob, this) ? Enumerable.Empty<string>() : PathElements.Skip(mount.VobDepth);
                result = subpathHandleProviderAction(subpathHandleProvider, parameter); // subpathHandleProvider.GetReadWriteHandleFromSubPath<T>(parameter);
            }
            else // ... otherwise, we have to get the handle from the reference.
            {
                var reference = ReferenceEquals(mount.Vob, this) ? mount.RootHandle.Reference : mount.RootHandle.Reference.GetChildSubpath(PathElements.Skip(mount.VobDepth));
                result = referenceToHandleAction(reference);
                //result = reference.ToReadWriteHandle<T>();
            }
            return result;
        }

        internal IReadWriteHandleBase<T> GetReadWriteHandleFromMount<T>(Mount mount)
        {
            return _GetHandleFromMount<T, ISubpathHandleProvider, IReadWriteHandleBase<T>>(mount,
                (shp, chunks) => shp.GetReadWriteHandleFromSubPath<T>(chunks), r => r.ToReadWriteHandle<T>());
        }
        internal IReadHandleBase<T> GetReadHandleFromMount<T>(Mount mount)
        {
            return _GetHandleFromMount<T, ISubpathHandleProvider, IReadHandleBase<T>>(mount,
                (shp, chunks) => shp.GetReadHandleFromSubPath<T>(chunks), r => r.ToReadHandle<T>());
        }
        internal IWriteHandleBase<T> GetWriteHandleFromMount<T>(Mount mount)
        {
            throw new NotImplementedException();
            //return _GetHandleFromMount<T, ISubpathHandleProvider, IWriteHandleBase<T>>(mount,
            //(shp, chunks) => shp.GetWriteHandleFromSubPath<T>(chunks), r => r.ToWriteHandle<T>());
        }

#if OLD
        /// <summary>
        /// Get the handle from the underlying mount (specified) representing this Vob's path and the specified type
        /// TOTEST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mount"></param>
        /// <returns></returns>
        private IReadWriteHandleBase<T> GetReadWriteHandleFromMount<T>(Mount mount)
        {
            IReadWriteHandleBase<T> result;

            // TODO TO_ASSERT mount path is a parent of this.Path

            //int mountDepthDelta = this.VobDepth - mount.VobDepth; // MICROOPTIMIZE - move to mount

#if true
            if (mount.RootHandle is ISubpathHandleProvider hp)
            {
                // (All that's needed here is IProvidesHandleOfDifferentType, but there is no interface for that yet.)

                // Some Handle types will be able to be smarter about providing handles....
                var parameter = Object.ReferenceEquals(mount.Vob, this) ? Enumerable.Empty<string>() : PathElements.Skip(mount.VobDepth);
                result = hp.GetReadWriteHandleFromSubPath<T>(parameter);
            }
            else
            {
                var reference = Object.ReferenceEquals(mount.Vob, this) ? mount.RootHandle.Reference : mount.RootHandle.Reference.GetChildSubpath(PathElements.Skip(mount.VobDepth));
                // ... otherwise, we have to get the handle from the reference.
                result = reference.ToReadWriteHandle<T>();
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
#endif

        /// <summary>
        /// The MountHandleObject itself is not significant -- it is just representing a handle to the root referene of the target Mount point
        /// </summary>
        /// <param name="mount"></param>
        /// <returns></returns>
        private IReadHandle<MountHandleObject> GetMountHandle(Mount mount)
        {
            IReadHandle<MountHandleObject> result;

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
                result = mount.Target.Path;
                l.Trace("UNTESTED: (alt) MountPath: " + result);
            }
            else
            {
                result = LionPath.Combine(mount.Target.Path, PathElements.Skip(mount.VobDepth)); // OPTIMIZE: cache this enumerable alongside the mount
                l.Trace("UNTESTED: MountPath: " + result);
            }

            return result;
        }

        #endregion

    }
}

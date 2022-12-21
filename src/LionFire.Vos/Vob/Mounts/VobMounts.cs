using LionFire.Referencing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Vos.Mounts
{

    public class VobMounts : VobNodeBase<VobMounts>
    {
        #region Options

        public VobMountOptions_Old Options { get; set; }

        #endregion

        #region Construction

        public VobMounts(Vob vob) : base(vob) { }

        #endregion

        #region Mounts

        public IEnumerable<IMount> AllMounts
        {
            get
            {
                if (ReadMounts != null) foreach (var m in ReadMounts?.Select(kvp => kvp.Value) ?? Enumerable.Empty<IMount>()) yield return m;
                if (WriteMounts != null) foreach (var m in WriteMounts?.Select(kvp => kvp.Value) ?? Enumerable.Empty<IMount>()) yield return m;
            }
        }

        #region HasMounts

        public bool HasLocalReadMounts => localReadMount != null || localReadMounts?.Any() == true;
        public bool HasLocalWriteMounts => localWriteMount != null || localWriteMounts?.Any() == true;

        public bool HasLocalMounts => HasLocalReadMounts || HasLocalWriteMounts;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mount"></param>
        /// <param name="force">
        ///  - If there are existing mount(s) and mount is exclusive, replace existing mount(s)
        /// Does not force:
        ///  - Overriding sealed on parent
        /// </param>
        public IMount Mount(IMount mount, bool force = false)
        {
            bool changedState = false;
            if (!mount.Options.ReadPriority.HasValue && !mount.Options.WritePriority.HasValue)
            {
                throw new ArgumentException("Either ReadPriority or WritePriority must have a value");
            }
            if (mount.Options.ReadPriority.HasValue && MountRead(mount, force)) changedState = true;
            if (mount.Options.WritePriority.HasValue && MountWrite(mount, force)) changedState = true;
            if (changedState) OnMountStateChanged();

            return mount;

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
        protected virtual void OnMountStateChanged()
        {
            MountStateVersion++;
            // FUTURE: Proactively invalidate and recalculate (at low thread priority) mount resolutions for child VobNodes.
        }
        public bool CanHaveMultiReadMounts { get; set; }
        public bool CanHaveMultiWriteMounts { get; set; }

        private IMount QueryExistingMount(IMount mount, ref IMount single, ref List<IMount> multi)
        {
            ArgumentNullException.ThrowIfNull(mount);
            if(single?.Target.Key == mount.Target.Key) { return single; }
            return multi?.Where(m => m.Target.Key == mount.Target.Key).FirstOrDefault();
        }

        public bool UpdateReadMount(IMount existing, IMount newMount)
        {
            if(!existing.Options.Equals(newMount.Options))
            {
                Unmount(existing.Target.Key);
                MountRead(newMount);
                return true;
            }
            if(existing.IsEnabled != newMount.IsEnabled)
            {
                existing.IsEnabled = newMount.IsEnabled;
                return true;
            }

            return false;
        }
                
        private bool MountRead(IMount mount, bool force = false) // DUPLICATE of MountWrite
        {
            var existing = QueryExistingMount(mount, ref localReadMount, ref localReadMounts);
            if (existing != null) { return UpdateReadMount(existing, mount); }

            #region Validation

            if (localReadMounts == null && localReadMount == null)
            {
                localReadMount = mount;
                return true;
            }
            if (localWriteMount != null && (localWriteMount.Options.IsExclusiveWithReadAndWrite || mount.Options.IsExclusiveWithReadAndWrite))
            {
                throw new VosException("Already has write mount, but either this mount or existing write mount has IsExclusiveWithReadAndWrite == true.");
            }
            if (localReadMount != null)
            {
                if (!CanHaveMultiReadMounts)
                {
                    throw new VosException("Already has read mount, but not allowed to have multiple read mounts because CanHaveMultiReadMounts is false.  First unmount the existing read mount.");
                }
                if (localReadMount.Options.IsExclusive || mount.Options.IsExclusive)
                {
                    throw new VosException("Already has read mount, but one or both mounts has IsExclusive == true.  First unmount one of the mounts, or disable the IsExclusive flag(s) before mounting.");
                }
            }

            #endregion


            if (localReadMount == null)
            {
                if (localReadMounts == null)
                {
                    localReadMount = mount;
                }
                else
                {
                    localReadMounts.Add(mount);
                }
            }
            else
            {
                var existingMount = localReadMount;
                localReadMounts = new List<IMount>
                {
                    existingMount,
                    mount
                };
                localReadMount = null;
            }

            return true;
        }


        /// <summary>
        /// Returns null on error or end (REVIEW)
        /// </summary>
        private struct MountEnumerable : IEnumerable<IMount>
        {
            private PersistenceDirection direction;
            private IVobNode<VobMounts> vobNode;
            public MountEnumerable(IVobNode<VobMounts> vobNode, PersistenceDirection read)
            {
                this.vobNode = vobNode;
                this.direction = read;
            }

            public IEnumerator<IMount> GetEnumerator() => new MountEnumerator(vobNode, direction);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            internal class MountEnumerator : IEnumerator<IMount>
            {
                int localIndex = 0;
                int parentIndex = 0;

                IVobNode<VobMounts> local;
                IVobNode<VobMounts> parent;
                PersistenceDirection direction;

                public MountEnumerator(IVobNode<VobMounts> local, PersistenceDirection direction)
                {
                    this.local = local;
                    parent = local.NextAncestor();
                    this.direction = direction;
                }

                public IMount Current { get; private set; }

                object IEnumerator.Current => Current;

                public void Dispose()
                {
                    local = null;
                    parent = null;
                }

                private void OnInvalid()
                {

                }
                public bool MoveNext()
                {
                    //if (parent == null)
                    //{
                    //    Current = null;
                    //    return false;
                    //    //return true;
                    //}
                    if (!ReferenceEquals(local.NextAncestor(), parent))
                    {
                        Current = null;
                        throw new Exception("!ReferenceEquals(local.ParentVobNode, parent)");
                        //OnInvalid();
                        //return true;
                        return false;
                    }
                    return direction switch
                    {
                        PersistenceDirection.Read => _MoveNext(firstIsPriority_Read),
                        PersistenceDirection.Write => _MoveNext(firstIsPriority_Write),
                        _ => throw new ArgumentException(nameof(direction)),
                    };
                }


                private bool firstIsPriority_Read(IMount local, IMount parent) => local.Options.ReadPriority > parent.Options.ReadPriority;
                private bool firstIsPriority_Write(IMount local, IMount parent) => local.Options.WritePriority > parent.Options.WritePriority;
                private bool _MoveNext(Func<IMount, IMount, bool> firstIsPriority)
                {
                    var nextLocal = NextLocal;
                    var nextParent = NextParent;

                    if (nextLocal == null && nextParent == null) return false;

                    if (nextLocal == null)
                    {
                        Current = nextParent;
                        parentIndex++;
                    }
                    else if (nextParent == null)
                    {
                        Current = nextLocal;
                        localIndex++;
                    }
                    else
                    {
                        if (firstIsPriority(nextLocal, nextParent))
                        {
                            Current = nextLocal;
                            localIndex++;
                        }
                        else
                        {
                            Current = nextParent;
                            parentIndex++;
                        }
                    }

                    return true;
                }

                IMount GetIndex(int index, IMount singleMount, List<IMount> mounts)
                {
                    if (index == 0)
                    {
                        if (singleMount != null && singleMount.IsEnabled) { return singleMount; }
                        else return mounts?.Where(m => m.IsEnabled).FirstOrDefault();
                    }
                    else return mounts?.Where(m => m.IsEnabled).ElementAtOrDefault(index);
                }

                IMount NextLocal
                 => direction == PersistenceDirection.Read
                            ? GetIndex(localIndex, local?.Value.localReadMount, local?.Value.localReadMounts)
                            : GetIndex(localIndex, local?.Value.localWriteMount, local?.Value.localWriteMounts);

                IMount NextParent
                  => direction == PersistenceDirection.Read
                    ? GetIndex(parentIndex, parent?.Value.localReadMount, parent?.Value.localReadMounts)
                    : GetIndex(parentIndex, parent?.Value.localWriteMount, parent?.Value.localWriteMounts);

                public void Reset()
                {
                    localIndex = 0;
                    parentIndex = 0;
                }
            }
        }

        public IEnumerable<IMount> RankedEffectiveReadMounts => new MountEnumerable(this, PersistenceDirection.Read);
        public IEnumerable<IMount> RankedEffectiveWriteMounts => new MountEnumerable(this, PersistenceDirection.Write);

        protected IEnumerable<IMount> AllEffectiveReadMounts
        {
            get
            {
                if (localReadMount != null) yield return localReadMount;
                if (localReadMounts != null) foreach (var m in localReadMounts) yield return m;
            }
        }

        protected IEnumerable<IMount> AllEffectiveWriteMounts
        {
            get
            {
                if (localWriteMount != null) yield return localWriteMount;
                if (localWriteMounts != null) foreach (var m in localWriteMounts) yield return m;
            }
        }

        private bool MountWrite(IMount mount, bool force = false) // DUPLICATE of MountRead
        {
            var existing = QueryExistingMount(mount, ref localReadMount, ref localReadMounts);
            if (existing != null) { return UpdateReadMount(existing, mount); }

            #region Validation

            if (localWriteMounts == null && localWriteMount == null)
            {
                localWriteMount = mount;
                return true;
            }
            if (localReadMount != null && (localWriteMount.Options.IsExclusiveWithReadAndWrite || mount.Options.IsExclusiveWithReadAndWrite))
            {
                throw new VosException("Already has read mount, but either this mount or existing write mount has IsExclusiveWithReadAndWrite == true.");
            }
            if (localWriteMount != null)
            {
                if (!CanHaveMultiWriteMounts)
                {
                    throw new VosException("Already has write mount, but not allowed to have multiple write mounts because CanHaveMultiWriteMounts is false.  First unmount the existing read mount.");
                }
                if (localWriteMount.Options.IsExclusive || mount.Options.IsExclusive)
                {
                    throw new VosException("Already has read mount, but one or both mounts has IsExclusive == true.  First unmount one of the mounts, or disable the IsExclusive flag(s) before mounting.");
                }
            }

            #endregion

            if (localWriteMount == null)
            {
                if (localWriteMounts == null)
                {
                    localWriteMount = mount;
                }
                else
                {
                    localWriteMounts.Add(mount);
                }
            }
            else
            {
                var existingMount = localWriteMount;
                localWriteMounts = new List<IMount>
                {
                    existingMount,
                    mount
                };
                localWriteMount = null;
            }

            return true;
        }

        public int Unmount(IReference mountTarget)
        {
            return Unmount(mountTarget.Key);
        }
        public int Unmount(string mountKey)
        {
            int unmountCount = 0;

            if (localReadMount?.Target.Key == mountKey)
            {
                localReadMount = null;
                unmountCount++;
            }

            if (localReadMounts != null)
            {
                var mount = localReadMounts.FirstOrDefault(m => m.Target.Key == mountKey);
                if (mount != null)
                {
                    localReadMounts.Remove(mount);
                    unmountCount++;
                }
            }

            if (localWriteMount?.Target.Key == mountKey)
            {
                localWriteMount = null;
                unmountCount++;
            }

            if (localWriteMounts != null)
            {
                var mount = localWriteMounts.FirstOrDefault(m => m.Target.Key == mountKey);
                if (mount != null)
                {
                    localWriteMounts.Remove(mount);
                    unmountCount++;
                }
            }
            return unmountCount;
        }
        public int MountStateVersion { get; protected set; }

        public IMount localReadMount;
        public List<IMount> localReadMounts;

        public IMount localWriteMount;
        public List<IMount> localWriteMounts;

        #endregion

        #region ReadMountsCache

        public int ReadMountsVersion { get; set; } = 0;
        public IEnumerable<KeyValuePair<int, IMount>> ReadMounts
        {
            get
            {
                var cache = ReadMountsCache;
                if (cache == null)
                {
                    var list = AllEffectiveReadMounts.ToList();
                    readMountsCache = list.Count switch
                    {
                        1 => new SingleMountResolutionCache(Vob, ReadMountsVersion++, list[0]),
                        0 => null,
                        _ => new MultiMountResolutionCache(Vob, ReadMountsVersion++, AllEffectiveReadMounts, PersistenceDirection.Read),
                    };
                }
                return readMountsCache?.Mounts;
            }
        }
        IMountResolutionCache ReadMountsCache => (readMountsCache != null && (this.NextAncestor() == null || readMountsCache.Version == ParentValue.MountStateVersion)) ? readMountsCache : null;
        IMountResolutionCache readMountsCache;

        #endregion

        #region Write Mounts Cache

        public int WriteMountsVersion { get; set; } = 0;

        public IEnumerable<KeyValuePair<int, IMount>> WriteMounts
        {
            get
            {
                var cache = WriteMountsCache;
                if (cache == null)
                {
                    var list = AllEffectiveWriteMounts.ToList();
                    writeMountsCache = list.Count switch
                    {
                        1 => new SingleMountResolutionCache(Vob, WriteMountsVersion++, list[0]),
                        0 => null,
                        _ => new MultiMountResolutionCache(Vob, WriteMountsVersion++, AllEffectiveWriteMounts, PersistenceDirection.Write),
                    };
                }
                return writeMountsCache?.Mounts;
            }
        }

        IMountResolutionCache WriteMountsCache => (writeMountsCache != null && (this.NextAncestor() == null || writeMountsCache.Version == ParentValue.MountStateVersion)) ? writeMountsCache : null;
        IMountResolutionCache writeMountsCache;

        #endregion


        //internal void OnMountsChangedFor(Vob changedVob, INotifyCollectionChangedEventArgs<Mount> e)
        //{
        //    throw new NotImplementedException(); // Still needed?
        //    //foreach (var vobWithMounts in VobsWithMounts.ToArray())
        //    //{
        //    //    if (changedVob.IsAncestorOf(vobWithMounts))
        //    //    {
        //    //        vobWithMounts.OnAncestorMountsChanged(changedVob, e);
        //    //    }
        //    //}
        //}

        // OLD
        //public bool HasMounts
        //{
        //    get => hasMounts;
        //    private set
        //    {
        //        if (hasMounts == value)
        //        {
        //            return;
        //        }

        //        hasMounts = value;

        //        if (hasMounts)
        //        {
        //            NextVobNode.VobsWithMounts.Add(this);
        //        }
        //        else
        //        {
        //            NextVobNode.VobsWithMounts.Remove(this);
        //        }
        //        InitializeEffectiveMounts();
        //    }
        //}
        //private bool hasMounts;

        //private void UpdateHasMounts() => HasMounts = mounts != null && mounts.Count > 0;
    }
}

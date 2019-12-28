using LionFire.Referencing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Vos.Mounts
{

    public class VobMountCache : VobDecoratorBase<VobMountCache>
    {

    }

    public class VobMounts : VobNodeBase<VobMounts>
    {

        public VobMounts(Vob vob ) : base(vob) { }

        #region Mounts

        #region HasMounts

        public bool HasLocalReadMounts => localReadMount != null || localReadMounts?.Any() == true;
        public bool HasLocalWriteMounts => localWriteMount != null || localWriteMounts?.Any() == true;

        public bool HasLocalMounts => HasLocalReadMounts || HasLocalWriteMounts;

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
        public Mount Mount(Mount mount, bool force = false)
        {
            bool changedState = false;
            if (!mount.MountOptions.ReadPriority.HasValue && !mount.MountOptions.WritePriority.HasValue)
            {
                throw new ArgumentException("Either ReadPriority or WritePriority must have a value");
            }
            if (mount.MountOptions.ReadPriority.HasValue && MountRead(mount, force)) changedState = true;
            if (mount.MountOptions.WritePriority.HasValue && MountWrite(mount, force)) changedState = true;
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
        public bool CanHaveMultiReadMounts { get { throw new NotImplementedException(); } }
        public bool CanHaveMultiWriteMounts { get { throw new NotImplementedException(); } }

        private bool MountRead(Mount mount, bool force = false)
        {
            #region Validation

            if (localReadMounts == null && localReadMount == null)
            {
                localReadMount = mount;
                return true;
            }
            if (localWriteMount != null && (localWriteMount.MountOptions.IsExclusiveWithReadAndWrite || mount.MountOptions.IsExclusiveWithReadAndWrite))
            {
                throw new VosException("Already has write mount, but either this mount or existing write mount has IsExclusiveWithReadAndWrite == true.");
            }
            if (localReadMount != null)
            {
                if (!CanHaveMultiReadMounts)
                {
                    throw new VosException("Already has read mount, but not allowed to have multiple read mounts.  First unmount the existing read mount.");
                }
                if (localReadMount.MountOptions.IsExclusive || mount.MountOptions.IsExclusive)
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
                localReadMounts = new List<Mount>
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
        private struct MountEnumerable : IEnumerable<Mount>
        {
            private PersistenceDirection direction;
            private IVobNode<VobMounts> vobNode;
            public MountEnumerable(IVobNode<VobMounts> vobNode, PersistenceDirection read)
            {
                this.vobNode = vobNode;
                this.direction = read;
            }

            public IEnumerator<Mount> GetEnumerator() => new MountEnumerator(vobNode, direction);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            internal class MountEnumerator : IEnumerator<Mount>
            {
                int localIndex = 0;
                int parentIndex = 0;

                IVobNode<VobMounts> local;
                IVobNode<VobMounts> parent;
                PersistenceDirection direction;

                public MountEnumerator(IVobNode<VobMounts> local, PersistenceDirection direction)
                {
                    this.local = local;
                    parent = local.ParentVobNode;
                    this.direction = direction;
                }

                public Mount Current { get; private set; }

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
                    if(parent == null)
                    {
                        Current = null;
                        return true;
                    }
                    if (!ReferenceEquals(local.ParentVobNode, parent))
                    {
                        Current = null;
                        OnInvalid();
                        return true;
                    }
                    return direction switch
                    {
                        PersistenceDirection.Read => _MoveNext(firstIsPriority_Read),
                        PersistenceDirection.Write => _MoveNext(firstIsPriority_Write),
                        _ => throw new ArgumentException(nameof(direction)),
                    };
                }


                private bool firstIsPriority_Read(Mount local, Mount parent) => local.MountOptions.ReadPriority > parent.MountOptions.ReadPriority;
                private bool firstIsPriority_Write(Mount local, Mount parent) => local.MountOptions.WritePriority > parent.MountOptions.WritePriority;
                private bool _MoveNext(Func<Mount, Mount, bool> firstIsPriority)
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

                Mount GetIndex(int index, Mount singleMount, List<Mount> mounts)
                {
                    if (index == 0)
                    {
                        if (singleMount != null) { return singleMount; }
                        else return mounts?.FirstOrDefault();
                    }
                    else return mounts[index];
                }

                Mount NextLocal
                 => direction == PersistenceDirection.Read
                            ? GetIndex(localIndex, local.Value.localReadMount, local.Value.localReadMounts)
                            : GetIndex(localIndex, local.Value.localWriteMount, local.Value.localWriteMounts);

                Mount NextParent
                  => direction == PersistenceDirection.Read
                    ? GetIndex(parentIndex, parent.Value.localReadMount, parent.Value.localReadMounts)
                    : GetIndex(parentIndex, parent.Value.localWriteMount, parent.Value.localWriteMounts);

                public void Reset()
                {
                    localIndex = 0;
                    parentIndex = 0;
                }
            }
        }

        public IEnumerable<Mount> RankedEffectiveReadMounts => new MountEnumerable(this, PersistenceDirection.Read);
        public IEnumerable<Mount> RankedEffectiveWriteMounts => new MountEnumerable(this, PersistenceDirection.Write);

        protected IEnumerable<Mount> AllEffectiveReadMounts
        {
            get
            {
                if (localReadMount != null) yield return localReadMount;
                if (localReadMounts != null) foreach (var m in localReadMounts) yield return m;
            }
        }

        protected IEnumerable<Mount> AllEffectiveWriteMounts
        {
            get
            {
                if (localWriteMount != null) yield return localWriteMount;
                if (localWriteMounts != null) foreach (var m in localWriteMounts) yield return m;
            }
        }

        private bool MountWrite(Mount mount, bool force = false)
        {
            throw new NotImplementedException();
        }

        public void Unmount(string mountKey)
        {
            throw new NotImplementedException();
        }
        public int MountStateVersion { get; protected set; }

        public Mount localReadMount;
        public List<Mount> localReadMounts;

        public Mount localWriteMount;
        public List<Mount> localWriteMounts;

        #endregion

        #region ReadMountsCache

        public int ReadMountsVersion { get; set; } = 0;
        public IEnumerable<KeyValuePair<int, Mount>> ReadMounts
        {
            get
            {
                var cache = ReadMountsCache;
                if (cache == null)
                {
                    var list = AllEffectiveReadMounts.ToList();
                    switch (list.Count)
                    {
                        case 1:
                            readMountsCache = new SingleMountResolutionCache(Vob, ReadMountsVersion++, list[0]);
                            break;
                        case 0:
                            readMountsCache = null;
                            break;
                        default:
                            readMountsCache = new MultiMountResolutionCache(Vob, ReadMountsVersion++, AllEffectiveReadMounts, PersistenceDirection.Read);
                            break;
                    }
                }
                return readMountsCache.Mounts;
            }
        }
        IMountResolutionCache ReadMountsCache => (readMountsCache != null && (ParentVobNode == null || readMountsCache.Version == ParentValue.MountStateVersion)) ? readMountsCache : null;
        IMountResolutionCache readMountsCache;

        #endregion

        #region Write Mounts Cache

        public int WriteMountsVersion { get; set; } = 0;

        public IEnumerable<KeyValuePair<int, Mount>> WriteMounts
        {
            get
            {
                throw new NotImplementedException();
                //if (writeMountsCache == null)
                //{

                //}
                //return writeMountsCache;
            }
        }

        //IMountResolutionCache writeMountsCache;

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

    }
}

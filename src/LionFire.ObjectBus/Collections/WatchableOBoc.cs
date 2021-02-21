using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Collections;

namespace LionFire.ObjectBus.Handles
{
    // TODO: Make this extrinsic?

#if UNUSED
    public abstract class SyncableOBoc : ROBoc<IReference<object>, object, OBaseCollectionEntry>
    {
        public SyncableOBoc() { }
        public SyncableOBoc(IReference reference) : base(reference) { }
    }
#endif

    public abstract class WatchableOBoc<TReference, T, TListEntry> : ROBoc<TReference, T, TListEntry>, IWatchableRC<T, TListEntry>
        where TReference :  IReference<T>, IReference<INotifyingReadOnlyCollection<TListEntry>>
        where T : INotifyingReadOnlyCollection<TListEntry>
        where TListEntry : OBaseCollectionEntry
    {

        #region Construction

        public WatchableOBoc() { }

        public WatchableOBoc(TReference reference) : base(reference)
        {
        }

        #endregion

        #region IsReadSyncEnabled

        /// <summary>
        /// If true, watch the underlying collection of items and keep this collection in sync.
        /// </summary>
        public abstract bool IsReadSyncEnabled { get; set; }

        #endregion
    }
}

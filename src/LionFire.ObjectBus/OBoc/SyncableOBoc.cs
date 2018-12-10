using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public abstract class SyncableOBoc : OBoc<object, OBaseCollectionEntry>
    {
        public SyncableOBoc() { }
        public SyncableOBoc(IReference reference) : base(reference) { }
    }

    public abstract class SyncableOBoc<T, TListEntry> : OBoc<T, TListEntry>, ISyncableRC<T, TListEntry>
        where TListEntry : OBaseCollectionEntry
    {

        #region Construction

        public SyncableOBoc() { }

        public SyncableOBoc(IReference reference) : base(reference)
        {
        }

        #endregion

        #region IsReadSyncEnabled

        /// <summary>
        /// If true, keep this collection in sync with the underlying collection of items.
        /// </summary>
        public abstract bool IsReadSyncEnabled { get; set; }

        #endregion
    }
}

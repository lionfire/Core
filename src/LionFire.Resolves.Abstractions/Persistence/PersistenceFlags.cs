using System;

namespace LionFire
{
    //public enum PersistenceStateFlags
    //{
    //    None,
    //    Pending = 1 << 1,
    //    Watching = 1 << 2,
    //    Autosave = 1 << 3,
    //    PendingDelete = 1 << 15
    //    Faulted = 1 << 31,
    //}


    //// MERGE - is this somewhere else?
    //// TODO TOIMPLEMENT
    //// MOVE
    //public enum SyncOptionFlags 
    //{

    //    ExplicitCreate,
    //    NoRecreate,

    //    DeleteBeatsUpdate,
    //    UpdateBeatsDelete,
    //}

    //public enum ResolveState
    //{

    //}

    [Flags]
    [Ignore]
    public enum PersistenceFlags
    {
        None = 0,

        /// <summary>
        /// If true, a retrieve or commit can be attempted
        /// REVIEW TODO - offload this to CanRead and CanWrite properties?
        /// </summary>
        PrimaryKeyValid = 1 << 0,

        #region Create

        /// <summary>
        /// If true, a Create can be attempted, either because Object is not default, or default values are allowed.
        /// (Some specialized handles may require some properties be set before create is allowoed) 
        /// </summary>
        CanCreate = 1 << 1,

        /// <summary>
        /// 
        /// </summary>
        OutgoingCreatePending = 1 << 2,

        IncomingCreateAvailable = 1 << 21, // TODO REORDER

        #endregion

        #region Retrieve & Change notification

        /// <summary>
        /// Object was read, or Object was written, and since then, there are have been no pending changes 
        /// to the local copy of the Object, and no known changes from the underlying data store.
        /// (Note that this instance may not be able to detect any changes, or may not be able to detect deep changes to the lcoal copy of the Object, nor may it be able to be notified when the underlying store retrieves an updated Object.)
        /// 
        /// If not UpToDate, state must have one of the following flags:
        ///  - IncomingCreationAvailable (Object was known not to exist, or not yet retrieved, and the underlying data store reported the Object was created at the Primary Key)
        ///  - IncomingRetrieveAvailable (it's not known whether the Object exists at the Primary Key)
        ///  - IncomingUpdateAvailable
        ///  - IncomingDeleteAvailable (it's known the Object existed, but it has been deleted.)
        ///  - OutgoingChangeAvailable
        ///  - OutgoingDeletePending
        ///  
        /// May also have one of these flags:
        ///  - NotFound, if Object == default
        /// </summary>
        UpToDate = 1 << 3,

        /// <summary>
        /// Happens after primary key is set, but the state of the Object in the store is unknown (no events have been retrieved from the underlying store.)
        /// </summary>
        IncomingRetrieveAvailable = 1 << 4,

        NotFound = 1 << 6,
        Found = 1 << 23,

        #endregion

        #region Update

        /// <summary>
        /// Changes available from underlying store
        /// (Happens after source-side change detection is enabled and a modification is detected.)
        /// </summary>
        IncomingUpdateAvailable = 1 << 7,

        OutgoingUpdatePending = 1 << 8,

        #endregion

        #region Delete

        /// <summary>
        /// A delete has been staged
        /// </summary>
        OutgoingDeletePending = 1 << 9,

        IncomingDeleteAvailable = 1 << 10,

        #endregion

        #region Merge status

        NeedsMergeAnalysis = 1 << 12,

        MergeConflict = 1 << 13, // Out of sync
        AutoMergeable = 1 << 14,  // Out of sync

        InSync = 1 << 11,

        #endregion

        #region Reachable Status

        /// <summary>
        /// Confirmed target is reachable (even if nothing was found there)
        /// </summary>
        Reachable = 1 << 19,

        /// <summary>
        /// Target is not reachable.  The connection string may be invalid or the host may not be available, or the process may not be running.
        /// </summary>
        Unreachable = 1 << 20,

        #endregion


        //ReadableStates = HasUnretrievedUpdates | NotRetrievedYet,
        //WritableStates = OutgoingCreatePending | OutgoingUpdatePending | MarkedForDeletion | NeedsMergeAnalysis | MergeConflict | AutoMergeable,

        OutgoingUpsertPending = 1 << 22,

        OutgoingFlags = OutgoingCreatePending | OutgoingDeletePending | OutgoingUpdatePending | OutgoingUpsertPending,
        IncomingFlags = IncomingCreateAvailable | IncomingDeleteAvailable | IncomingUpdateAvailable,
    }

    public static class PersistenceStateExtensions
    {

        public static PersistenceFlags ForgetIncomingAvailable(this PersistenceFlags state)
             => state & ~(
                    PersistenceFlags.IncomingCreateAvailable
                    | PersistenceFlags.IncomingRetrieveAvailable
                    | PersistenceFlags.IncomingUpdateAvailable
                    | PersistenceFlags.IncomingDeleteAvailable
                    );

        /// <summary>
        /// Does not reset:
        ///  - Reachable/Unreachable
        ///  - Incoming changes detected
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static PersistenceFlags AfterDiscard(this PersistenceFlags state)
             => state & ~(PersistenceFlags.OutgoingCreatePending
                    | PersistenceFlags.UpToDate
                    | PersistenceFlags.OutgoingUpdatePending
                    | PersistenceFlags.OutgoingDeletePending
                    | PersistenceFlags.NeedsMergeAnalysis
                    | PersistenceFlags.MergeConflict
                    | PersistenceFlags.AutoMergeable
                    | PersistenceFlags.InSync
                    );
    }
}

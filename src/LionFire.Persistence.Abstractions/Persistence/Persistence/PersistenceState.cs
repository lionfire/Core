using System;

namespace LionFire
{

    [Flags]
    [Ignore]
    public enum PersistenceState
    {
        None = 0,
        /// <summary>
        /// If true, a retrieve or commit can be attempted
        /// </summary>
        PrimaryKeyValid = 1 << 0,

        #region Create

        /// <summary>
        /// If true (object is not null), a create can be attempted.  
        /// (Some specialized handles may require some properties be set before create is allowoed) 
        /// </summary>
        CanCreate = 1 << 1,

        NotCreatedYet = 1 << 9,

        #endregion

        #region Retrieve & Change notification

        /// <summary>
        /// Object is believed to exist in the source, either because Create was committed, or write/update was done, or a previously persisted item was moved to this location.
        /// Example usage: do not auto-save if client has not saved it yet.  (But maybe auto-save to a temp location anyway?)
        /// </summary>
        Persisted = 1 << 15,

        /// <summary>
        /// Happens after primary key is set, or source-side change detection is enabled and a modification is detected.
        /// </summary>
        RetrieveAvailable = 1 << 3,

        /// <summary>
        /// Happens after source-side change detection is enabled and a modification is detected.
        /// </summary>
        SourceModified = 1 << 4,

        #endregion

        #region Update

        /// <summary>
        /// Updates available from source
        /// </summary>
        UpdatesAvailable = 1 << 7,

        HasUnsavedChanges = 1 << 8,

        #endregion

        #region Delete

        /// <summary>
        /// A delete has been staged
        /// </summary>
        DeletePending = 1 << 5,

        DeletedAtSource = 1 << 16,

        #endregion

        #region Merge status

        NeedsMergeAnalysis = 1 << 12,

        MergeConflict = 1 << 13, // Out of sync
        AutoMergeable = 1 << 14,  // Out of sync

        InSync = 1 << 18,

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
        //WritableStates = NotCreatedYet | HasUnsavedChanges | MarkedForDeletion | NeedsMergeAnalysis | MergeConflict | AutoMergeable,

    }
}

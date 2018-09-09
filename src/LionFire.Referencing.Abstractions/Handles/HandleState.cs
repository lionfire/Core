using System;

namespace LionFire
{
    public enum HandleEvents
    {
        None = 0,

        StateChanged = 1 << 0,

        ObjectReferenceChangedByHandle = 1 << 1,
        ObjectReferenceChangedBySource = 1 << 2,

        ObjectPropertiesChangedViaObject = 1 << 3,
        ObjectPropertiesChangedFromSource = 1 << 4,

        ObjectCommittedToSource = 1 << 3,
        ObjectRetrievedFromSource = 1 << 3,

        CommittedObjectCreate = 1 << 0,
        CommittedObjectDelete = 1 << 0,

        ObjectDeletedInSource = 1 << 0,

        ObjectForgotten = 1 << 10,


        HandleDisposed = 1 << 12,
    }

    [Flags]
    [Ignore]
    public enum HandleState
    {
        None = 0,
        /// <summary>
        /// If true, a retrieve or commit can be attempted
        /// </summary>
        PrimaryKeyValid = 1 << 0,

        #region Create

        /// <summary>
        /// If true (object is not null), a create can be attempted
        /// </summary>
        CanCreate = 1 << 1,

        #endregion

        #region Retrieve & Change notification

        /// <summary>
        /// Has been retrieved from source.  If null, retrieve operation succeeded but found nothing.
        /// </summary>
        Retrieved = 1 << 2,

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

        #endregion

        #region Delete

        /// <summary>
        /// A delete has been staged
        /// </summary>
        DeletePending = 1 << 5,

        #endregion


        /// <summary>
        /// Updates available from source
        /// </summary>
        UpdatesAvailable = 1 << 7,

        HasUnsavedChanges = 1 << 8,

        NotCreatedYet = 1 << 9,

        HasUnretrievedUpdates = 1 << 10,

        MarkedForDeletion = 1 << 11,

        NeedsMergeAnalysis = 1 << 12,
        MergeConflict = 1 << 13,
        AutoMergeable = 1 << 14,

        /// <summary>
        /// Object is believed to exist in the source, either because Create was committed, or a 
        /// </summary>
        Persisted = 1 << 15,

        //ReadableStates = HasUnretrievedUpdates | NotRetrievedYet,
        //WritableStates = NotCreatedYet | HasUnsavedChanges | MarkedForDeletion | NeedsMergeAnalysis | MergeConflict | AutoMergeable,
    }
}

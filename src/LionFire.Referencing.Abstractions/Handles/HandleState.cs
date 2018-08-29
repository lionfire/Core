namespace LionFire
{
    public enum HandleState
    {
        /// <summary>
        /// If true, a retrieve or commit can be attempted
        /// </summary>
        PrimaryKeyValid,

        #region Create
        
        /// <summary>
        /// If true (object is not null), a create can be attempted
        /// </summary>
        CanCreate,

        #endregion

        #region Retrieve & Change notification

        /// <summary>
        /// Has been retrieved from source.  If null, retrieve operation succeeded but found nothing.
        /// </summary>
        Retrieved,

        /// <summary>
        /// Happens after primary key is set, or source-side change detection is enabled and a modification is detected.
        /// </summary>
        RetrieveAvailable,

        /// <summary>
        /// Happens after source-side change detection is enabled and a modification is detected.
        /// </summary>
        SourceModified,

        #endregion

        #region Update

        #endregion

        #region Delete


        // Delete

        /// <summary>
        /// A delete has been requested
        /// </summary>
        DeletePending,

        /// <summary>
        /// Happens after primary key is set, or source-side change detection is enabled and a modification is detected.
        /// </summary>
        SourceDeleted,

        #endregion


        /// <summary>
        /// Updates available from source
        /// </summary>
        UpdatesAvailable,

        HasUnsavedChanges,

        NotCreatedYet,


        HasUnretrievedUpdates,

        MarkedForDeletion,

        NeedsMergeAnalysis,
        MergeConflict,
        AutoMergeable,


        //ReadableStates = HasUnretrievedUpdates | NotRetrievedYet,
        //WritableStates = NotCreatedYet | HasUnsavedChanges | MarkedForDeletion | NeedsMergeAnalysis | MergeConflict | AutoMergeable,
    }
}

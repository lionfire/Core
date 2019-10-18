using System;

namespace LionFire.Persistence
{
    /// <summary>
    /// Subset of PersistenceResultFlags
    /// </summary>
    [Flags]
    public enum RetrieveResultFlags
    {
        None = 0,

        /// <summary>
        /// True if operation was successful, even if retrieve did not find anything
        /// </summary>
        Success = PersistenceResultFlags.Success,

        /// <summary>
        /// Exception
        /// </summary>
        Fail = PersistenceResultFlags.Fail,

        Found = PersistenceResultFlags.Found,
        NotFound = PersistenceResultFlags.NotFound,

        Retrieved = PersistenceResultFlags.Retrieved,

        RetrievedNull = PersistenceResultFlags.RetrievedNullOrDefault,

        /// <summary>
        /// When checking for whether an operation is possible, this is set if the operation is expected to succeed.
        /// </summary>
        PreviewSuccess = PersistenceResultFlags.PreviewSuccess,

        /// <summary>
        /// When checking for whether an operation is possible, this is set if the operation is expected to fail.
        /// </summary>
        PreviewFail = PersistenceResultFlags.PreviewFail,

        //PreviewNotFound = 1 << 22, // Not used yet. Should it be?

        ProviderNotAvailable = PersistenceResultFlags.ProviderNotAvailable,

        Noop = PersistenceResultFlags.Noop,
    }
}
using System;

namespace LionFire.Persistence
{
    /// <summary>
    /// Subset of TransferResultFlags
    /// </summary>
    [Flags]
    public enum RetrieveResultFlags
    {
        None = 0,

        /// <summary>
        /// True if operation was successful, even if retrieve did not find anything
        /// </summary>
        Success = TransferResultFlags.Success,

        /// <summary>
        /// Exception
        /// </summary>
        Fail = TransferResultFlags.Fail,

        Found = TransferResultFlags.Found,
        NotFound = TransferResultFlags.NotFound,

        Retrieved = TransferResultFlags.Retrieved,

        RetrievedNull = TransferResultFlags.RetrievedNullOrDefault,

        /// <summary>
        /// When checking for whether an operation is possible, this is set if the operation is expected to succeed.
        /// </summary>
        PreviewSuccess = TransferResultFlags.PreviewSuccess,

        /// <summary>
        /// When checking for whether an operation is possible, this is set if the operation is expected to fail.
        /// </summary>
        PreviewFail = TransferResultFlags.PreviewFail,

        //PreviewNotFound = 1 << 22, // Not used yet. Should it be?

        ProviderNotAvailable = TransferResultFlags.ProviderNotAvailable,

        Noop = TransferResultFlags.Noop,
    }
}
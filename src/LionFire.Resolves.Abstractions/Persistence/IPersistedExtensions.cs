namespace LionFire.Persistence
{
    public static class IPersistedExtensions
    {
        public static bool NotFound(this IPersists has) => has.Flags.HasFlag(PersistenceFlags.NotFound);
        public static bool RetrievedNullOrDefault(this IPersists has) => has.Flags.HasFlag(PersistenceFlags.UpToDate);

        public static bool IsSuccess(this TransferResultFlags flags) => flags.HasFlag(TransferResultFlags.Success) || flags.HasFlag(TransferResultFlags.PreviewSuccess);
        public static bool? IsSuccessTernary(this TransferResultFlags flags)
        {
            if (flags.HasFlag(TransferResultFlags.Success) || flags.HasFlag(TransferResultFlags.PreviewSuccess)) return true;
            if (flags.HasFlag(TransferResultFlags.Fail) || flags.HasFlag(TransferResultFlags.PreviewFail)) return false;
            /// if (flags.HasFlag(TransferResultFlags.PreviewIndeterminate)) return null; // Redundant
            return null;
        }

    }
}
namespace LionFire.Persistence
{
    public static class IPersistedExtensions
    {
        public static bool NotFound(this IPersists has) => has.Flags.HasFlag(PersistenceFlags.NotFound);
        public static bool RetrievedNullOrDefault(this IPersists has) => has.Flags.HasFlag(PersistenceFlags.UpToDate);

        public static bool IsSuccess(this PersistenceResultFlags flags) => flags.HasFlag(PersistenceResultFlags.Success) || flags.HasFlag(PersistenceResultFlags.PreviewSuccess);
        public static bool? IsSuccessTernary(this PersistenceResultFlags flags)
        {
            if (flags.HasFlag(PersistenceResultFlags.Success) || flags.HasFlag(PersistenceResultFlags.PreviewSuccess)) return true;
            if (flags.HasFlag(PersistenceResultFlags.Fail) || flags.HasFlag(PersistenceResultFlags.PreviewFail)) return false;
            /// if (flags.HasFlag(PersistenceResultFlags.PreviewIndeterminate)) return null; // Redundant
            return null;
        }

    }
}
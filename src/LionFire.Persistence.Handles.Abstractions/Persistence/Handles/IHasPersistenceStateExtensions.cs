namespace LionFire.Persistence
{
    public static class IHasPersistenceStateExtensions
    {
        public static bool IsPersisted(this IPersists state) => state.Flags.HasFlag(PersistenceFlags.UpToDate);
    }
}

namespace LionFire.Persistence
{
    public static class IHasPersistenceStateExtensions
    {
        public static bool IsPersisted(this IPersisted state) => state.State.HasFlag(PersistenceState.UpToDate);
    }
}

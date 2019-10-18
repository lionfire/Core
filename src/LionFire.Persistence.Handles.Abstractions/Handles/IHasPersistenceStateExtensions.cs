namespace LionFire.Persistence
{
    public static class IHasPersistenceStateExtensions
    {
        public static bool IsPersisted(this IHasPersistenceState state) => state.State.HasFlag(PersistenceState.UpToDate);
    }
}

namespace LionFire.Persistence
{
    public interface IPersistenceSnapshot<out T>
    {
        bool HasValue { get; }
        PersistenceState State { get; }
        T Value { get; }
    }
}
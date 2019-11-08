namespace LionFire.Persistence
{
    public interface IPersistenceSnapshot<out T>
    {
        PersistenceFlags Flags { get; }
        bool HasValue { get; }
        T Value { get; }
    }

    
}
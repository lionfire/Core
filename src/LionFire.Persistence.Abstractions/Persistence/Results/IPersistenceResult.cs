namespace LionFire.Persistence
{
    public interface IPersistenceResult
    {
        object Error { get; }

        PersistenceResultFlags Flags { get; set; }
    }

}
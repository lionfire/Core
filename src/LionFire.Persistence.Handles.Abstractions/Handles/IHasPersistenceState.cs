namespace LionFire.Persistence
{
    public interface IHasPersistenceState
    {
        PersistenceState State { get; }
        event PersistenceStateChangeHandler StateChanged;


    }
}

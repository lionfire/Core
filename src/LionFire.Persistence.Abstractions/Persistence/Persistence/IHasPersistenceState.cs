namespace LionFire.Persistence
{
    public interface IHasPersistenceState
    {
        PersistenceState State { get; }
    }

    public interface IHasPersistenceStateEvents
    {
        event PersistenceStateChangeHandler StateChanged;
    }

    public delegate void PersistenceStateChangeHandler(PersistenceState from, PersistenceState to);

}

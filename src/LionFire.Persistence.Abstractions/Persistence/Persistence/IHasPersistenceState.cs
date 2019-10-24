namespace LionFire.Persistence
{

    public interface IHasPersistenceStateEvents
    {
        event PersistenceStateChangeHandler StateChanged;
    }

    public delegate void PersistenceStateChangeHandler(PersistenceState from, PersistenceState to);

}

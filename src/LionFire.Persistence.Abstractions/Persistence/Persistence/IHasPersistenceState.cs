namespace LionFire.Persistence
{

    public interface IHasPersistenceStateEvents
    {
        event PersistenceStateChangeHandler StateChanged;
    }

    public delegate void PersistenceStateChangeHandler(PersistenceFlags from, PersistenceFlags to);

}

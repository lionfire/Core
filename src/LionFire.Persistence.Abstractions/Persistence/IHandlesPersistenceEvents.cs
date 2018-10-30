

namespace LionFire.Persistence
{
    /// <summary>
    /// Implement this in your object to be notified when a persistence mechanism (such as ObjectBus / Vos) persists or does some sort of persistence operation on your object.
    /// </summary>
    public interface IHandlesPersistenceEvents
    {
        // TODO: Is there a better way of doing this?
        void OnPersistenceEvent(PersistenceEvent ev);
    }


}
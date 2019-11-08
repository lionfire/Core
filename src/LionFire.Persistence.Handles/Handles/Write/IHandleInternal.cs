namespace LionFire.Persistence.Handles
{
    internal interface IHandleInternal<TValue> : INotifyPersists<TValue>
    {
        object persistenceLock { get; }
        bool HasValue { get; }
        PersistenceFlags Flags { get; set; }
        TValue ProtectedValue { get; set; }
        void RaisePersistenceStateChanged(PersistenceEvent<TValue> ev);
    }
}

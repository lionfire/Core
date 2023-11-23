namespace LionFire.Persistence.Handles;

internal interface INotifyingHandleInternal<TValue> : IHandleInternal<TValue>, INotifyPersists<TValue>, INotifyPersistsInternal<TValue>
{
    object PersistenceLock { get; }
    
    //PersistenceFlags Flags { get; set; }
    //void RaisePersistenceStateChanged(PersistenceEvent<TValue> ev);
}

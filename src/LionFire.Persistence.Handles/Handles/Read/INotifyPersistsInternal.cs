namespace LionFire.Persistence.Handles
{
    internal interface INotifyPersistsInternal<TValue> : IPersists<TValue>
        //where TValue : class
    {
        void RaisePersistenceEvent(PersistenceEvent<TValue> ev);
        
    }
}

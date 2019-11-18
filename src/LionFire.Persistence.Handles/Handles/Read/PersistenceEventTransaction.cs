using System;

namespace LionFire.Persistence.Handles
{
    internal sealed class PersistenceEventTransaction<TValue> : IDisposable
            //where TValue : class
    {
        IPersistenceSnapshot<TValue> Old { get; set; }
        INotifyPersists<TValue> Sender { get; set; }
        public PersistenceEventTransaction(INotifyPersists<TValue> sender)
        {
            Sender = sender;
            Old = sender.PersistenceState;
        }
        public void Dispose()
        {

            ((INotifyPersistsInternal<TValue>)Sender).RaisePersistenceEvent(new PersistenceEvent<TValue>
            {
                Sender = Sender,
                Old = Old,
                New = Sender.PersistenceState
            });
        }
    }
}

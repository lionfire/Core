using LionFire.Events;
using System;

namespace LionFire.Persistence
{
    public struct PersistenceChangeEvent<T> : IValueChanged<IPersistenceSnapshot<T>>
    {
        public IPersisted Sender { get; set; }

        public PersistenceEventKind Kind { get; set; }

        public IPersistenceSnapshot<T> NewValue { get; set; }

        public IPersistenceSnapshot<T> OldValue { get; set; }

    }

    public interface INotifyingPersisted<T> : IPersisted
    {
        event EventHandler<IValueChanged<IPersistenceSnapshot<T>>> StateChanged;
    }
    
}

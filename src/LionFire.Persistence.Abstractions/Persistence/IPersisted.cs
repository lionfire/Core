using LionFire.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public interface IPersisted
    {
        PersistenceState State { get; }
    }
    public interface IPersisted<T> : IPersisted
    {
        event EventHandler<IValueChanged<IPersistenceSnapshot<T>>> StateChanged;
    }
}

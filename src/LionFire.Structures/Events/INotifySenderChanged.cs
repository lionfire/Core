using System;

namespace LionFire.Events
{
    public interface INotifySenderChanged<TValue>
    {
        event EventHandler<IValueChanged<TValue>> SenderChanged;
    }
}

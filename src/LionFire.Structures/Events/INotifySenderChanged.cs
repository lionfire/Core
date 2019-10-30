using System;

namespace LionFire.Events
{
    public interface INotifySenderChanged<out TValue>
    {
        event EventHandler<IValueChanged<TValue>> SenderChanged;
    }
}

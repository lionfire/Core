using System;

namespace LionFire.Events
{
    public interface INotifiesSenderValueChanged<TValue>
    {
        event EventHandler<ValueChanged<TValue>> ValueChanged;
    }
}

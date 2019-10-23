using System;

namespace LionFire.Events
{
    public interface INotifiesValueChanged<TValue>
    {
        event Action<ValueChanged<TValue>> ValueChanged;
    }
}

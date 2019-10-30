using System;

namespace LionFire.Events
{
    // This should be an extension method that may require DI to adapt to all the possible ways objects may notify of their values being changed (e.g. INPC, INCC)
    //public interface INotifyValueContentsChanged<out TValue>
    //{
    //    event Action<TValue> ValueContentsChanged;
    //}
    

    public interface INotifyChanged<out TValue>
    {
        event Action<IValueChanged<TValue>> Changed;
    }
}

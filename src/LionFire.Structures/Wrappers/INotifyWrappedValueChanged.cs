using System;

namespace LionFire.Structures
{
    public interface INotifyWrappedValueChanged
    {
        event Action<INotifyWrappedValueChanged> WrappedValueChanged;
    }

    public interface INotifyWrappedValueReplaced
    {
        event Action<INotifyWrappedValueReplaced,object,object> WrappedValueForFromTo;
    }
}

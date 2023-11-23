using System;

namespace LionFire.Structures;

// REVIEW - Replace this with ReactiveUI's ReactiveObject events?

public interface INotifyWrappedValueChanged
{
    event Action<INotifyWrappedValueChanged> WrappedValueChanged;
}

public interface INotifyWrappedValueReplaced
{
    event Action<INotifyWrappedValueReplaced,object,object> WrappedValueForFromTo;
}

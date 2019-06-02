using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LionFire.Events
{
    /// <summary>
    /// A genericized interface for listening to property change events
    /// </summary>
    public interface IPropertyChangedListener
    {
        object Target { get; set; }
        bool IsTargetSupported { get; }

        event Action<PropertyInfo> PropertyChanged;
        event Action<PropertyInfo, object> PropertyChangedTo;

        bool HasFastChangedTo { get; }
        bool HasChangedFrom { get; }

        //event Action<object> ChangedTo;
        //event Action<object> ChangedFrom;
        //event Action<object, object> ChangedFromTo;

    }
}

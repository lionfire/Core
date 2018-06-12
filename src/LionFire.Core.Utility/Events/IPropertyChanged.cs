using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    public interface IPropertyChanged
    {
        event Action<string> PropertyValueChanged;
    }

    public interface IPropertyIdChanged // TODO OPTIMIZE - use this for network
    {
        event Action<int> PropertyValueChangedForId;
    }
}

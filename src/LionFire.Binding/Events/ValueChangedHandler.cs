using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    [Obsolete("Use EventHandler<ValueChanged<TValue>> or Action<ValueChanged<TValue>>")]
    public delegate void ValueChangedHandler<T>(T oldValue, T newValue);

}

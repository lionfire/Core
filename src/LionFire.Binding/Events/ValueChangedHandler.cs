using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    [Obsolete("Use EventHandler<ValueChanged<T>> or Action<ValueChanged<T>>")]
    public delegate void ValueChangedHandler<T>(T oldValue, T newValue);

}

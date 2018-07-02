using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    public delegate void ValueChangedHandler<T>(T oldValue, T newValue);
}

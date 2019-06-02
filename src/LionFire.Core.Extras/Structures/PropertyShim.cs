using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Structures
{
    public class PropertyShim
    {
        public object Property
        {
            get { return Getter(); }
            set { Setter(value); }
        }
        public Func<object> Getter;
        public Action<object> Setter;

        public PropertyShim(Func<object> getter, Action<object> setter) { this.Getter = getter; this.Setter = setter; }
    }
}

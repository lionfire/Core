using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    /// <remarks>
    /// To avoid deriving from a base class here, implement IContainsMultiTyped isntead.
    /// Consider deriving from MultiTypeContainer instead for a more direct inheritance?  This approach relies upon Extension Methods from IContainsMultiTypes.
    /// </remarks>
    public class ContainsMultiTyped : IContainsMultiTyped
    {
        public MultiType MultiTyped
        {
            get
            {
                if (multiTyped== null) { multiTyped = new MultiType(); }
                return multiTyped;
            }
            protected set { multiTyped = value; }
        }
        public MultiType multiTyped;
    }
}

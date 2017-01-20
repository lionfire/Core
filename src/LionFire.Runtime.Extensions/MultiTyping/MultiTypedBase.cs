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
    public class MultiTypedBase : IContainsMultiTyped
    {
        public MultiType MultiTyped { get; protected set; } = new MultiType();
    }
}

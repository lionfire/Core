using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    /// <remarks>
    /// To avoid deriving from a base class here, implement IContainsMultiTyped isntead.
    /// </remarks>
    public class MultiTypedBase : IContainsMultiTyped
    {
        public MultiTypeContainer MultiTyped { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    /// <summary>
    /// Convenience base class for a class implementing IMultiTypable
    /// </summary>
    /// <remarks>
    /// To avoid deriving from a base class here, implement IContainsMultiTyped instead.
    /// Consider deriving from MultiTypeContainer instead for a more direct inheritance?  This approach relies upon Extension Methods from IContainsMultiTypes.
    /// </remarks>
    public class MultiTypable : IMultiTypable
    {
        public IMultiTyped MultiTyped
        {
            get
            {
                if (multiTyped == null)
                {
                    //LazyInitializer.EnsureInitialized(ref multiTyped);
                    Interlocked.CompareExchange(ref multiTyped, new MultiType(), null);
                }
                return multiTyped;
            }
            protected set => multiTyped = value;
        }
        public IMultiTyped multiTyped;
    }
}

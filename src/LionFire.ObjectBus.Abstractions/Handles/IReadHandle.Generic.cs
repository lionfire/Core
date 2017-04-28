using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{

    public interface IReadHandleEx<
#if !UNITY
        out // Crashes unity???
#endif
T> : IReadHandle
        where T : class
    {
        T ObjectField { get; } // REVIEW - this seems to be a hack.  eliminate it?
    }


#if !AOT
    /// <summary>
    /// Covariant version of IHandle&lt;T&gt;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadHandle<
        #if !UNITY
        out // Crashes unity???
#endif
T> : IReadHandleEx<T>, IReadHandle
        where T : class
    {
        new T Object { get; }
        
    }
#endif
}

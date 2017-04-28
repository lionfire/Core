using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public interface IHasReadHandle
    {
        IReadHandle ReadHandle { get; }
    }

#if !AOT
    public interface IHasReadHandle<
#if !UNITY
out
#endif
 T> : IHasReadHandle
        where T : class
    {
        new IReadHandle<T> ReadHandle { get; }
    }
#endif


}

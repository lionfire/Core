using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ObjectBus
{
#if !AOT
    public interface IHasHandle<T>
        //: IHasHandle
        where T : class
    {
        IHandle<T> Handle { get; }
    }
#endif

}

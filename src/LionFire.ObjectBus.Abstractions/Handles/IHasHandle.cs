using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{

    public interface IHasHandle
    {
        
        IHandle Handle { get;  }
    }

    public interface IHasHandleSetter
    {
        IHandle Handle { set; }
    }

	#if !AOT
    public interface IHasHandle<T>
        //: IHasHandle
        where T : class
    {
        IHandle<T> Handle { get; }
    }
#endif

}

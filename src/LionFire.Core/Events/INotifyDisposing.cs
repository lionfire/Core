using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    public interface INotifyDisposing<T>
        where T : class//, INotifyDisposing<T>
    {
#if AOT
		event EventHandler Disposing;
#else
        event Action<T> Disposing;
#endif
    }

     public interface INotifyDisposing
    {
        event Action<object> Disposing;
    }
}

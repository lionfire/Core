#if TODO // Needed?
using System.Windows.Threading;

namespace LionFire.Threading
{
    public class WpfDispatcherProvider : IDispatcherProvider
    {
        public IDispatcher DispatcherFor(object obj)
        {
            var dobj = obj as DispatcherObject;
            if (dobj == null) return null;
            return new WpfDispatcherWrapper(dobj.Dispatcher);
        }
    }

}
#endif

using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LionFire.Threading
{
    public class WpfDispatcherWrapper : IDispatcher
    {
        Dispatcher dispatcher;
        public WpfDispatcherWrapper(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public bool IsInvokeRequired => !dispatcher.CheckAccess();

        public void Invoke(Delegate p, params object[] args)
        {
            dispatcher.Invoke(p, args);
        }
        public Task BeginInvoke(Delegate p, params object[] args)
        {
            DispatcherOperation dop = dispatcher.BeginInvoke(p, args);
            return dop.Task;
        }
    }
}

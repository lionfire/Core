using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using LionFire.Threading;

namespace LionFire.UI.Wpf
{
    public class WindowsDispatcherWrapper : IDispatcher
    {
        Dispatcher dispatcher;
        public WindowsDispatcherWrapper(Dispatcher dispatcher) { this.dispatcher = dispatcher; }
        public bool CheckAccess() => dispatcher.CheckAccess();

        public bool IsInvokeRequired => !dispatcher.CheckAccess();

        public void Invoke(Action action) => dispatcher.Invoke(action);

        public void Invoke(Delegate p, params object[] args)
        {
            dispatcher.Invoke(p, args);
        }

        public Task BeginInvoke(Delegate p, params object[] args)
        {
            return dispatcher.BeginInvoke(p, args).Task;
        }
    }
    public static class WindowsDispatcherExtensions
    {
        public static IDispatcher ToIDispatcher(this Dispatcher dispatcher)
        {
            return new WindowsDispatcherWrapper(dispatcher);
        }
    }
}

using System;
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

    }
    public static class WindowsDispatcherExtensions
    {
        public static IDispatcher ToIDispatcher(this Dispatcher dispatcher)
        {
            return new WindowsDispatcherWrapper(dispatcher);
        }
    }
}

using System;
using System.Windows.Threading;

namespace LionFire.UI.Wpf
{
    public class WindowsDispatcherWrapper : IDispatcher
    {
        Dispatcher dispatcher;
        public WindowsDispatcherWrapper(Dispatcher dispatcher) { this.dispatcher = dispatcher; }
        public bool CheckAccess() => dispatcher.CheckAccess();

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

using LionFire.Threading;
using System;
using System.Threading.Tasks;


namespace LionFire.Dispatching
{

    // ENH: Allow queuing actions before this instance is available -- buffer them in another class and hold them until StrideDispatcher arrives
    public class StrideDispatcherAccessor : IDispatcher
    {
        StrideDispatcher Implementation => _implementation ??= StrideDispatcher.Instance;
        StrideDispatcher _implementation;

        public bool IsInvokeRequired => Implementation.IsInvokeRequired;

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException
        {
            add
            {
                Implementation.UnhandledException += value;
            }

            remove
            {
                Implementation.UnhandledException -= value;
            }
        }

        public Task BeginInvoke(Action action) => Implementation.BeginInvoke(action);
        public Task<object> BeginInvoke(Func<object> func) => Implementation.BeginInvoke(func);
        public void Invoke(Action action) => Implementation.Invoke(action);
        public object Invoke(Func<object> func) => Implementation.Invoke(func);
    }
}

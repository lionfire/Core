using System;

namespace LionFire.Threading
{
    // TODO: Make this more like the WPF event args
    public sealed class DispatcherUnhandledExceptionEventArgs : EventArgs
    {
        public DispatcherUnhandledExceptionEventArgs() { }
        public DispatcherUnhandledExceptionEventArgs(object dispatcher, Exception exception)
        {
            Dispatcher = dispatcher;
            Exception = exception;
        }

#if CSharp9
        public object Dispatcher { get; init; }
        public Exception Exception { get; init; }
#else
        public object Dispatcher { get; set; }
        public Exception Exception { get; set; }
#endif
        public bool Handled { get; set; }
    }
}

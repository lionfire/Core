#if UNUSED
using LionFire.Dependencies;
using LionFire.Threading;
using System;
using System.Threading.Tasks;


namespace LionFire.Dispatching
{
    public class ExceptionDispatcher : IDispatcher
    {

        public bool IsInvokeRequired => throw new DependencyMissingException();

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public Task BeginInvoke(Action action) => throw new DependencyMissingException();
        public Task<object> BeginInvoke(Func<object> func) => throw new DependencyMissingException();
        public void Invoke(Action action) => throw new DependencyMissingException();
        public object Invoke(Func<object> func) => throw new DependencyMissingException();
    }
}
#endif
using LionFire.Threading;
using System;
using System.Threading.Tasks;

namespace LionFire.Dispatching
{
    /// <summary>
    /// Drop-in Replacement for Windows Dispatcher (and Noesis's limited Dispatcher)
    /// 
    /// using Dispatcher = LionFire.Dispatching.UnityThreadDispatcherWrapper
    /// </summary>
    public class UnityThreadDispatcherWrapper : IDispatcher
    {
        public bool IsInvokeRequired => throw new NotImplementedException();

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public Task BeginInvoke(Action action) => throw new NotImplementedException();
        public Task<object> BeginInvoke(Func<object> func) => throw new NotImplementedException();
        public bool CheckAccess() => true; // TODO

        public Task<T> Invoke<T>(Func<T> func)
        {
            return UnityDispatcher.Execute(func);
        }

        public void Invoke(Action action) => throw new NotImplementedException();
        public object Invoke(Func<object> func) => throw new NotImplementedException();
    }
}

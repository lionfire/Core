using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Threading
{
    public class DefaultDispatcher : IDispatcher
    {
        public bool IsInvokeRequired => false;

        public event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;

        public virtual void Invoke(Action action)
        {
            try
            {
                action();
            } catch(Exception ex)
            {
                var uh = new DispatcherUnhandledExceptionEventArgs(this, ex);
                UnhandledException?.Invoke(this, uh);
                if (!uh.Handled)
                {
                    throw;
                }
            }
        }

        public virtual object Invoke(Func<object> func) => func();

        public virtual Task BeginInvoke(Action action)
        {
            return Task.Run(action);
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(x => action())); // OLD
        }
        public virtual Task<object> BeginInvoke(Func<object> func)
        {
            return Task.Run(func);
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(x => action())); // OLD
        }
        public void Invoke(Delegate p, params object[] args) => throw new NotImplementedException();
        Task IDispatcher.BeginInvoke(Action action) => throw new NotImplementedException();
    }
}

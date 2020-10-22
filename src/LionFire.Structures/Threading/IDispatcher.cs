
using System;
using System.Threading.Tasks;

namespace LionFire.Threading
{
    public interface IDispatcher
    {
        bool IsInvokeRequired { get; }
        //void Invoke(Delegate p, params object[] args); // From where?
        //Task BeginInvoke(Delegate p, params object[] args);

        // From ILionFireShell - TODO
        void Invoke(Action action);
        object Invoke(Func<object> func);
        Task BeginInvoke(Action action);
        Task<object> BeginInvoke(Func<object> func);

        event EventHandler<DispatcherUnhandledExceptionEventArgs> UnhandledException;
    }
    
}

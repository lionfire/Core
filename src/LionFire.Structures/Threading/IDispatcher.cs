
using System;
using System.Threading.Tasks;

namespace LionFire.Threading
{
    public interface IDispatcher
    {
        bool IsInvokeRequired { get; }
        void Invoke(Delegate p, params object[] args);
        Task BeginInvoke(Delegate p, params object[] args);
    }


    public static class IDispatcherExtensions
    {
        public static void Invoke(this IDispatcher dispatcher, Action a)
        {
            dispatcher.Invoke(new Action(a));
        }
        public static bool CheckAccess(this IDispatcher dispatcher)
        {
            return dispatcher.IsInvokeRequired;
        }
    }
}

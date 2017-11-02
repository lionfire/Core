
using System;

namespace LionFire.Threading
{
    public interface IDispatcher
    {
        bool IsInvokeRequired { get; }
        void Invoke(Action p);
    }

    public static class IDispatcherExtensions
    {
        public static bool CheckAccess(this IDispatcher dispatcher)
        {
            return dispatcher.IsInvokeRequired;
        }
    }
}


using System;

namespace LionFire.Threading
{

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


using System;
using System.Threading.Tasks;

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
            return !dispatcher.IsInvokeRequired;
        }

        public static Task InvokeAsync(this IDispatcher dispatcher, Action action)
        {
            if (!dispatcher.IsInvokeRequired) { action(); return Task.CompletedTask; }

            var tcs = new TaskCompletionSource<object>();

            dispatcher.BeginInvoke(() =>
            {
                action();
                tcs.SetResult(null);
            }).FireAndForget();

            return tcs.Task;
        }
    }
}

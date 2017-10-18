using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        // MOVE to some sort of task supervisor?
        public static event Action<Exception> SwallowedException;
        public static void SwallowException(this Task task)
        {
            if (task.Exception != null) SwallowedException?.Invoke(task.Exception);

            task.ContinueWith(_ => { return; });
        }
        public static void FireAndForget(this Task task, string taskName = null)
        {
        }

        // From http://stackoverflow.com/a/40927835/208304
        public static T GetResultSafe<T>(this Task<T> task)
        {
            if (SynchronizationContext.Current == null)
                return task.Result;

            if (task.IsCompleted)
                return task.Result;

            var tcs = new TaskCompletionSource<T>();
            task.ContinueWith(t =>
            {
                var ex = t.Exception;
                if (ex != null)
                    tcs.SetException(ex);
                else
                    tcs.SetResult(t.Result);
            }, TaskScheduler.Default);

            return tcs.Task.Result;
        }

        // Adapted from http://stackoverflow.com/a/40927835/208304
        public static void WaitSafe(this Task task)
        {
            if (SynchronizationContext.Current == null)
                return;

            if (task.IsCompleted)
                return;

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
            {
                var ex = t.Exception;
                if (ex != null)
                    tcs.SetException(ex);
                else
                    tcs.SetResult(null);
            }, TaskScheduler.Default);

            return;
        }
    }
}

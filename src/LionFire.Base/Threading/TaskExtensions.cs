using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Threading
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
        public static void FireAndForget(this Task _) { }

        public static void FireAndForget(this Task _, string taskName)
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


        #region Timeout

        // TODO: Add more for single and multiple tasks

        // Based on https://stackoverflow.com/a/22865384/208304
        // See that question for examples with multiple tasks
        //public async static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        //{
        //    if (await (Task.WhenAny(task, Task.Delay(timeout))) != task) throw new TimeoutException();
        //    return await task;
        //}
        //public async static Task WithTimeout(this Task task, TimeSpan timeout)
        //{
        //    if (await (Task.WhenAny(task, Task.Delay(timeout))) != task) throw new TimeoutException();
        //}
        public async static Task WithTimeout(this Task task, int? milliseconds, Action onException = null)
        {
            if (!milliseconds.HasValue || milliseconds <= 0) return;

            if (await (Task.WhenAny(task, Task.Delay(TimeSpan.FromMilliseconds(milliseconds.Value)))) != task) (onException ?? DefaultTimeoutException)();
        }
        private static void DefaultTimeoutException() => throw new TimeoutException();
        
        #endregion
    }
}

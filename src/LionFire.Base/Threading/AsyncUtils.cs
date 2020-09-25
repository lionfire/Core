using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Threading
{
    // REVIEW - consider moving to LionFire.Base

    public static class AsyncUtils
    {

        /// <summary>
        /// CAUTION: Does not work for Mutexes
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="millisecondsTimeout"></param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// Based on https://thomaslevesque.com/2015/06/04/async-and-cancellation-support-for-wait-handles/
        /// Also see AsyncEx library.
        /// </remarks>
        /// <returns></returns>
        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout = -1, CancellationToken cancellationToken = default)
        {
            RegisteredWaitHandle registeredHandle = null;
            CancellationTokenRegistration tokenRegistration = default;
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    tcs,
                    millisecondsTimeout,
                    true);
                tokenRegistration = cancellationToken.Register(
                    state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                    tcs);
                return await tcs.Task;
            }
            finally
            {
                if (registeredHandle != null)
                    registeredHandle.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken)
        {
            return handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
        }
    }
}

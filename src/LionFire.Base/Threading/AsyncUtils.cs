using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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


    // Retrieved on 201102 from https://medium.com/@cilliemalan/how-to-await-a-cancellation-token-in-c-cbfc88f28fa2 
    public static class AsyncExtensions
    {
        /// <summary>
        /// Allows a cancellation token to be awaited.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static CancellationTokenAwaiter GetAwaiter(this CancellationToken ct)
        {
            // return our special awaiter
            return new CancellationTokenAwaiter
            {
                CancellationToken = ct
            };
        }

        /// <summary>
        /// The awaiter for cancellation tokens.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public struct CancellationTokenAwaiter : INotifyCompletion, ICriticalNotifyCompletion
        {
            public CancellationTokenAwaiter(CancellationToken cancellationToken)
            {
                CancellationToken = cancellationToken;
            }

            internal CancellationToken CancellationToken;

            public object GetResult()
            {
                // this is called by compiler generated methods when the
                // task has completed. Instead of returning a result, we 
                // just throw an exception.
                if (IsCompleted) throw new OperationCanceledException();
                else throw new InvalidOperationException("The cancellation token has not yet been cancelled.");
            }

            // called by compiler generated/.net internals to check
            // if the task has completed.
            public bool IsCompleted => CancellationToken.IsCancellationRequested;

            // The compiler will generate stuff that hooks in
            // here. We hook those methods directly into the
            // cancellation token.
            public void OnCompleted(Action continuation) =>
                CancellationToken.Register(continuation);
            public void UnsafeOnCompleted(Action continuation) =>
                CancellationToken.Register(continuation);
        }
    }
}

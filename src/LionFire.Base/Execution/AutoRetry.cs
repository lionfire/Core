using LionFire.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
        // TODO: Use this.  Maybe have a key/value system: AutoRetry("Filesystem") AutoRetry("Web")
    public class AutoRetrySettings 
    {
        public int DefaultMaxRetries = 50;
        public bool ThrowLastExceptionOnFail = true;
        public int DefaultMillisecondsBetweenAttempts = 200;

        #region Static

        static AutoRetrySettings() {
            defaultSettings = new AutoRetrySettings()
            {
            };
        }

        public static AutoRetrySettings Default => defaultSettings;
        static readonly AutoRetrySettings defaultSettings;
        
        #endregion

    }

    public static class AutoRetryExtensions
    {
        private const int DefaultMaxRetries = 15;
        private const bool ThrowLastExceptionOnFail = true;
        private const int DefaultMillisecondsBetweenAttempts = 150;

        // DUPLICATE - similar logic in 3 methods

#if !AOT
        public static async Task<TResult> AutoRetry<TResult>(this Func<Task<TResult>> taskGenerator, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts, Predicate<Exception> allowException = null)
#else
		[AotReplacement]
        public static object AutoRetry(this Func<object> action, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts)
#endif
        {
            if (maxRetries == 0) { return await taskGenerator().ConfigureAwait(false); }
            //if (millisecondsBetweenAttempts == int.MinValue) { millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts; } // UNUSED

            List<Exception> exceptions = null;
            for (int retriesRemaining = maxRetries; retriesRemaining >= 0; retriesRemaining--)
            {
                try
                {
                    return await taskGenerator().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (allowException != null && !allowException(ex)) throw;

                    if (ex is IPotentiallyTemporaryError pte && pte.IsTemporaryError == PotentiallyTemporaryErrorKind.KnownPermanent)
                    {
                        throw;
                    }

                    if (exceptions == null) exceptions = new List<Exception>();
                    exceptions.Add(ex);

                    if (millisecondsBetweenAttempts > 0)
                    {
                        await Task.Delay(millisecondsBetweenAttempts).ConfigureAwait(false);
                    }
                    Trace.WriteLine($"[autoretry] {retriesRemaining} retries remain after exception {ex.GetType().FullName}");
                }
            }
            if (throwLastExceptionOnFail)
            {
                throw exceptions.Last();
            }
            else
            {
                throw new Exception("Multiple retries failed and created the exceptions in the inner exception.", new AggregateException(exceptions));
            }
        }

#if !AOT
        public static async Task<TResult> AutoRetry<TResult>(this Func<TResult> action, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts, Predicate<Exception> allowException = null)
#else
		[AotReplacement]
        public static object AutoRetry(this Func<object> action, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts)
#endif
        {
            List<Exception> exceptions = null;
            for (int retriesRemaining = maxRetries; retriesRemaining >= 0; retriesRemaining--)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    if (allowException != null && !allowException(ex)) throw;

                    if(ex is IPotentiallyTemporaryError pte && pte.IsTemporaryError == PotentiallyTemporaryErrorKind.KnownPermanent)
                    {
                        throw;
                    }

                    if (exceptions == null) exceptions = new List<Exception>();
                    exceptions.Add(ex);

                    if (millisecondsBetweenAttempts > 0)
                    {
                        await Task.Delay(millisecondsBetweenAttempts).ConfigureAwait(false);
                    }
                    Trace.WriteLine($"[autoretry] {retriesRemaining} retries remain after exception {ex.GetType().FullName}");
                }
            }
            if (throwLastExceptionOnFail)
            {
                throw exceptions.Last();
            }
            else
            {
                throw new Exception("Multiple retries failed and created the exceptions in the inner exception.", new AggregateException(exceptions));
            }
        }



#if !AOT
        public static async Task AutoRetry(this Action action, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts, Predicate<Exception> allowException = null)
#else
		[AotReplacement]
        public static object AutoRetry(this Func<object> action, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts)
#endif
        {
            List<Exception> exceptions = null;
            for (int retriesRemaining = maxRetries; retriesRemaining >= 0; retriesRemaining--)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    if (allowException != null && !allowException(ex)) throw;

                    if (ex is IPotentiallyTemporaryError pte && pte.IsTemporaryError == PotentiallyTemporaryErrorKind.KnownPermanent)
                    {
                        throw;
                    }

                    if (exceptions == null) exceptions = new List<Exception>();
                    exceptions.Add(ex);

                    Debug.WriteLine($"AutoRetry got {ex.GetType().Name}, retrying after {millisecondsBetweenAttempts}ms.  {retriesRemaining} retries remaining. \r\n"+ex.ToString());
                    if (millisecondsBetweenAttempts > 0)
                    {
                        await Task.Delay(millisecondsBetweenAttempts).ConfigureAwait(false);
                    }
                    Trace.WriteLine($"[autoretry] {retriesRemaining} retries remain after exception {ex.GetType().FullName}");
                }
            }
            if (throwLastExceptionOnFail)
            {
                throw exceptions.Last();
            }
            else
            {
                throw new Exception("Multiple retries failed and created the exceptions in the inner exception.", new AggregateException(exceptions));
            }
        }
    }
}

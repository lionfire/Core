using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution
{
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

                    if (exceptions == null) exceptions = new List<Exception>();
                    exceptions.Add(ex);

                    if (millisecondsBetweenAttempts > 0)
                    {
                        await Task.Delay(millisecondsBetweenAttempts);
                    }
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

                    if (exceptions == null) exceptions = new List<Exception>();
                    exceptions.Add(ex);

                    Debug.WriteLine($"AutoRetry got {ex.GetType().Name}, retrying after {millisecondsBetweenAttempts}ms.  {retriesRemaining} retries remaining.");
                    if (millisecondsBetweenAttempts > 0)
                    {
                        await Task.Delay(millisecondsBetweenAttempts);
                    }
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

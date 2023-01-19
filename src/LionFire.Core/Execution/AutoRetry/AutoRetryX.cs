using LionFire.Exceptions;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;

namespace LionFire.Execution;

public static class AutoRetryX
{
    #region Telemetry

    private static readonly Meter Meter = new Meter("LionFire.Execution.AutoRetry", "1.0");
    private static readonly Counter<long> AutoRetryFailC = Meter.CreateCounter<long>("AutoRetryFail");
    private static readonly Counter<long> AutoRetryWaitC = Meter.CreateCounter<long>("AutoRetryWait", "ms", "How long AutoRetry is waiting between retry attempts");
    private static readonly Histogram<long> AutoRetrySuccessC = Meter.CreateHistogram<long>("AutoRetrySuccess");

    #endregion

    //private static ILogger l = Log.Get(typeof(AutoRetryX).FullName);  // TODO: Replace Debug statements?

    private const int DefaultMaxRetries = 20;
    private const bool ThrowLastExceptionOnFail = true;
    private const int DefaultMillisecondsBetweenAttempts = 100;

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
                    AutoRetryWaitC.IncrementWithContext(millisecondsBetweenAttempts);
                    await Task.Delay(millisecondsBetweenAttempts).ConfigureAwait(false);
                }
                Trace.WriteLine($"[autoretry] {retriesRemaining} retries remain after exception {ex.GetType().FullName}", typeof(AutoRetryX).FullName);
            }
            finally
            {
                AutoRetrySuccessC.RecordWithContext(maxRetries - retriesRemaining);
            }
        }

        AutoRetryFailC.IncrementWithContext();

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
                Trace.WriteLine($"{retriesRemaining} retries remain after exception {ex.GetType().FullName}", typeof(AutoRetryX).FullName);
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

                Debug.WriteLine($"AutoRetry got {ex.GetType().Name}, retrying after {millisecondsBetweenAttempts}ms.  {retriesRemaining} retries remaining. \r\n" + ex.ToString(), typeof(AutoRetryX).FullName);
                if (millisecondsBetweenAttempts > 0)
                {
                    await Task.Delay(millisecondsBetweenAttempts).ConfigureAwait(false);
                }
                Trace.WriteLine($"{retriesRemaining} retries remain after exception {ex.GetType().FullName}", typeof(AutoRetryX).FullName);
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

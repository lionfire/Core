using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution;

public class AutoRetryContext
{
    public int DefaultMaxRetries = 15;
    public bool ThrowLastExceptionOnFail = true;
    public int DefaultMillisecondsBetweenAttempts = 150;

    public Predicate<Exception> AllowException;

    #region Static

    static AutoRetryContext()
    {
        defaultSettings = new AutoRetryContext()
        {
        };
    }

    public static AutoRetryContext Default => defaultSettings;
    static readonly AutoRetryContext defaultSettings;

    #endregion


#if !AOT
    public async Task<TResult> AutoRetry<TResult>(Func<TResult> action, int maxRetries = int.MinValue, bool? throwLastExceptionOnFail = null, int millisecondsBetweenAttempts = int.MinValue, Predicate<Exception> allowException = null)
#else
		[AotReplacement]
    public static object AutoRetry(this Func<object> action, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts)
#endif
    {
        return await AutoRetryX.AutoRetry<TResult>(action, maxRetries == int.MinValue ? DefaultMaxRetries : maxRetries,
            throwLastExceptionOnFail.HasValue ? throwLastExceptionOnFail.Value : ThrowLastExceptionOnFail,
            millisecondsBetweenAttempts == int.MinValue ? DefaultMillisecondsBetweenAttempts : millisecondsBetweenAttempts,
            allowException ?? AllowException).ConfigureAwait(false);

    }

#if !AOT
    public async Task AutoRetry(Action action, int maxRetries = int.MinValue, bool? throwLastExceptionOnFail = null, int millisecondsBetweenAttempts = int.MinValue, Predicate<Exception> allowException = null)
#else
		[AotReplacement]
    public static object AutoRetry(this Func<object> action, int maxRetries = DefaultMaxRetries, bool throwLastExceptionOnFail = ThrowLastExceptionOnFail, int millisecondsBetweenAttempts = DefaultMillisecondsBetweenAttempts)
#endif
    {
        await AutoRetryX.AutoRetry(action, maxRetries == int.MinValue ? DefaultMaxRetries : maxRetries,
            throwLastExceptionOnFail.HasValue ? throwLastExceptionOnFail.Value : ThrowLastExceptionOnFail,
            millisecondsBetweenAttempts == int.MinValue ? DefaultMillisecondsBetweenAttempts : millisecondsBetweenAttempts,
            allowException ?? AllowException).ConfigureAwait(false);

    }
}

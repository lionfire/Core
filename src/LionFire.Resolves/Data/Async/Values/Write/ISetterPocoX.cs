using LionFire.Dependencies;
using LionFire.Results;
using LionFire.ExtensionMethods.Objects;
using LionFire.Data.Async.Sets;
using LionFire.Persistence;
using LionFire.Data;
using System.Linq;

namespace LionFire.ExtensionMethods.Poco.Data.Async;

public static class ISetterPocoX
{
    /// <summary>
    /// Put using a ISetter&lt;TKey, TValue&gt; service (or set of services) registered in DependencyContext.Current
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="returnFirstSuccess">If false (default), it will instead return first result that has IsSuccess().HasValue == true.  If true, it will try all ICommitters until one returns IsSuccess() == true.</param>
    /// <returns></returns>
    public static async Task<ISetResult<TValue>> SetViaAmbient<TKey, TValue>(this TKey key, TValue value, bool returnFirstSuccess = false)
    {
        foreach (var setter in key?.GetAmbientServiceProvider().GetServices<ISetter<TKey, TValue>>() ?? Enumerable.Empty<ISetter<TKey, TValue>>())
        {
            var result = await setter.Set(key, value).ConfigureAwait(false);
            if (!returnFirstSuccess)
            {
                if (result.IsSuccess().HasValue) return result;
            }
            else
            {
                if (result.IsSuccess == true) return result;
            }
        }
        return SetResult<TValue>.FailWithFlags(TransferResultFlags.PersisterNotAvailable);
    }
}

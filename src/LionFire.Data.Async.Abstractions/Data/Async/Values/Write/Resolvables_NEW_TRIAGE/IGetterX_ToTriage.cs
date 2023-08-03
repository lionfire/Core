using LionFire.Persistence;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using System;
using System.Threading.Tasks;
using LionFire.Data;

namespace LionFire.Resolvables;

public static class IGetterX_ToTriage
{
#if UNUSED // Useful?  Users of handles won't be using this 
    public static Func<object, bool?> DefaultHasValueResolver = o =>
    {
        if (o is ILazilyGets rex)
        {
            return rex.HasValue;
        }
        if (o is IDefaultable w)
        {
            return w.HasValue;
        }
        return null;
    };

    public static Func<object, bool?> HasValueResolver = DefaultHasValueResolver;

    public static bool HasValue(this IResolvable resolvable) => HasValueResolver(resolvable) == true;
#endif

    /// <summary>
    /// Always returns false  for IResolvable, unless the object is also IAsyncResolvable in which case the answer is deferred to IsResolutionExpensive(this IAsyncResolvable).
    /// </summary>
    public static bool IsResolutionExpensive(this IGetter getter) => getter is IGetterEx ar && ar.IsResolutionExpensive();

    public static async Task<T> GetValueAsync<T>(this IStatelessGetter<T> resolves)
    {
        var result = await resolves.Get().ConfigureAwait(false);
        if (result.IsSuccess != true) throw new TransferException("Resolve failed: " + result.ToString());
        return result.Value;
    }
}

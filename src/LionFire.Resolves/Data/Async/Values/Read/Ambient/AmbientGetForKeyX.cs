using LionFire.Dependencies;
using LionFire.ExtensionMethods.Objects;
using System.Linq;
using LionFire.Data;

namespace LionFire.ExtensionMethods.Poco.Resolvables;

public static class IResolverPocoExtensions
{
    public static async Task<TValue?> AmbientGetValue<TKey, TValue>(this TKey resolvable, bool returnFirstFound = true) => (await AmbientGet<TKey, TValue>(resolvable, returnFirstFound).ConfigureAwait(false)).Value;

    /// <summary>
    /// Get using a IResolver&lt;TKey, TValue&gt; service (or set of services) registered in DependencyContext.Current
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="resolvable"></param>
    /// <param name="returnFirstFound">If false, it will instead return first success (which may have HasValue == false)</param>
    /// <returns></returns>
    public static async Task<IGetResult<TValue>> AmbientGet<TKey, TValue>(this TKey resolvable, bool returnFirstFound = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resolvable);

        bool gotResolver = false;
        foreach (var resolver in resolvable!.GetAmbientServiceProvider()?.GetServices<IGetter<TKey, TValue>>() ?? Enumerable.Empty<IGetter<TKey, TValue>>())
        {
            gotResolver = true;
            var result = await resolver.Get(resolvable, cancellationToken).ConfigureAwait(false);
            if (returnFirstFound)
            {
                if (result.HasValue) return result;
            }
            else
            {
                if (result.IsSuccess == true) return result;
            }
        }

        if (!gotResolver) return RetrieveResult<TValue>.ProviderNotAvailable;

        return RetrieveResult<TValue>.SuccessNotFound;
    }
}

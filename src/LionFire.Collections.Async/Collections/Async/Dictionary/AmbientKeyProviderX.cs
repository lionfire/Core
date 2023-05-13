#nullable enable

using LionFire.Structures.Keys;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;
using LionFire.Structures;

namespace LionFire.Structures.Keys;

public static class AmbientKeyProviderX
{
    public static TKey GetKey<TKey, TValue>(TValue value)
        => GetKey<TKey, TValue>(value, null);

    public static TKey GetKey<TKey, TValue>(TValue value, IServiceProvider? serviceProvider )
    {
        var keyed = (value as IKeyed<TKey>);
        if (keyed != null) { return keyed.Key; }

        {
            throw new NotImplementedException("TODO: TryGetKey?");
            //var keyProvider = (serviceProvider ??= DependencyContext.Current?.ServiceProvider)?.GetService<IKeyProvider<TKey>>();
            //if (keyProvider != null)
            //{
            //    var result = keyProvider.TryGetKey(value);
            //    if (result.success) { return result.key!; }
            //}
        }

        {
            var keyGenerator = serviceProvider?.GetService<IKeyGenerator<TKey>>();
            if (keyGenerator != null)
            {
                var result = keyGenerator.TryGetKey(value);
                if (result.success) { return result.key!; }
            }
        }

        var msg = $"No {nameof(IKeyed<TValue>)} interface found, and either IKeyProviderService<TKey> and IKeyGenerator<TKey> are not registered or did not succeed in getting key for object of type " + value?.GetType().FullName;
        //Debug.WriteLine(msg);
        throw new NotImplementedException(msg);
        //return value!.GetHashCode().ToString(); // bad idea
    }
}


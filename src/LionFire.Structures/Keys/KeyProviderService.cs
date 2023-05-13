#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures.Keys;

public class KeyProviderService<TKey, TValue> : IKeyProvider<TKey, TValue>
{
    public KeyProviderService(IEnumerable<IKeyProviderStrategy<TKey, TValue>> strategies)
    {
        Strategies = strategies;
    }

    public IEnumerable<IKeyProviderStrategy<TKey, TValue>> Strategies { get; }

    public TKey GetKey(TValue? value)
    {
        var result = TryGetKey(value);
        if (result.success) return result.key;
        throw new NotFoundException();
    }

    public (bool success, TKey key) TryGetKey(TValue? obj)
    {
        if(obj is IKeyed<TKey> k) { return (true, k.Key); }

        foreach (var strategy in Strategies)
        {
            var result = strategy.TryGetKey(obj);
            if (result.success)
            {
                return result;
            }
        }
        return (false, default!); // NULLABILITY override
    }
}

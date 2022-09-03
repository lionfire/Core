using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures.Keys;

public class KeyProviderService<TKey> : IKeyProviderService<TKey>
{
    public KeyProviderService(IEnumerable<IKeyProvider<TKey>> keyProviders)
    {
        KeyProviders = keyProviders;
    }

    public IEnumerable<IKeyProvider<TKey>> KeyProviders { get; }

    public (bool, TKey?) TryGetKey(object obj)
    {
        if(obj is IKeyed<TKey> k) { return (true, k.Key); }

        foreach (var kp in KeyProviders)
        {
            var result = kp.TryGetKey(obj);
            if (result.success)
            {
                return result;
            }
        }
        return (false, default);
    }
}

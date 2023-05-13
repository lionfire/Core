#if UNUSED
#nullable enable

namespace LionFire.Structures.Keys;

// To register:
// .TryAddEnumerableSingleton<IKeyProviderStrategy<string, object>, KeyedKeyProviderStrategy<string, object>>()


// KeyProviderService does this on its own, making this redundant:
public class KeyedKeyProviderStrategy< TKey, TValue> : IKeyProviderStrategy<TKey, TValue>
{
    public (bool success, TKey? key) TryGetKey(TValue? value)
    {
        if(value is IKeyed<TKey> keyed) { return (true, keyed.Key); }
        return (false, default);
    }
}
#endif
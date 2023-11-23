#nullable enable

namespace LionFire.Structures.Keys;

// REVIEW - return Option<TKey> instead of TKey? ?

public interface IKeyProviderStrategy<TKey>: IKeyProviderStrategy<TKey, object>
{
}

public interface IKeyProviderStrategy<TKey, TValue>
{
    (bool success, TKey? key) TryGetKey(TValue? value);
}

#nullable enable

namespace LionFire.Structures.Keys;

public interface IKeyProvider<TKey>
{
    (bool success, TKey? key) TryGetKey(object? obj);
}

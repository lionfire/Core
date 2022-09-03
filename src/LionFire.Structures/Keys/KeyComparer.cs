using System.Collections.Generic;

namespace LionFire.Structures.Keys;

public class KeyComparer<TKey, TItem> : IComparer<TItem>
{
    public KeyComparer(IKeyProviderService<TKey> keyProviderService)
    {
        KeyProviderService = keyProviderService;
    }

    public IKeyProviderService<TKey> KeyProviderService { get; }

    public int Compare(TItem? x, TItem? y) => Comparer<TKey>.Default.Compare(KeyProviderService.TryGetKey(x).key, KeyProviderService.TryGetKey(y).key);
}

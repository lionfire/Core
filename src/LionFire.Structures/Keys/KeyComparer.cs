#nullable enable


namespace LionFire.Structures.Keys;

public class KeyComparer<TKey, TItem> : IComparer<TItem>
{
    public KeyComparer(IKeyProvider<TKey, TItem> keyProvider)
    {
        KeyProvider = keyProvider;
    }

    public IKeyProvider<TKey, TItem> KeyProvider { get; }

    public int Compare(TItem? x, TItem? y) => Comparer<TKey>.Default.Compare(KeyProvider.GetKey(x), KeyProvider.GetKey(y));
}

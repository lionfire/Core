namespace LionFire.Collections
{
    public interface IDictionaryCache<TKey, TValue>
    {
        TValue this[TKey key] { get; }
    }
}

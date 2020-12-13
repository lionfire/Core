namespace LionFire.Collections
{

    /// <summary>
    /// Contains a cache of ancestors of type TValue that are addressable by TKey.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IHasPathCache<TKey, TValue>
        where TValue : class
    {
        // TODO: Create ConcurrentWeakDictionaryCache, and IConcurrentDictionaryCache, and use the latter here.
        ConcurrentDictionaryCache<TKey, TValue> PathCache { get; }
    }
}

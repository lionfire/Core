using System.Collections.Generic;

namespace LionFire.Structures
{
    public static class IKeyedExtensions
    {
        public static void Add<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, TValue keyed)
            where TValue : IKeyed<TKey>
        {
            dictionary.Add(keyed.Key, keyed);
        }
    }
}

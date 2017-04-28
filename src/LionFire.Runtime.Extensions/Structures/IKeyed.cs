using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface IKeyed<TKey>
    {
        TKey Key { get; }
    }

    public static class IKeyedExtensions
    {
        public static void Add<TKey,TValue>(this Dictionary<TKey,TValue> dictionary, TValue keyed)
            where TValue : IKeyed<TKey>
        {
            dictionary.Add(keyed.Key, keyed);
        }
    }
}

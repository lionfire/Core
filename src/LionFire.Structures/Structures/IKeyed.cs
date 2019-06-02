using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public interface IKeyed<TKey>
    {
        TKey Key { get; }
    }

    /// <summary>
    /// Writable version of IKeyed
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IKeyedW<TKey> : IKeyed<TKey>
    {
        new TKey Key { get; set; }
    }
}

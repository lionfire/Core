#if AOT
using System.Collections;
#endif

namespace LionFire.Structures
{

    /// <summary>
    /// Read-only interface mirroring KeyValuePair with the benefit of covariance
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IKeyValuePair<out TKey, out TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
    }
}

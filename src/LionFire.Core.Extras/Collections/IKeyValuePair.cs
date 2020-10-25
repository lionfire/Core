#if AOT
using System.Collections;
#endif

namespace LionFire // RENAME LionFire.ExtensionMethods
{

    public interface IKeyValuePair<out TKey, out TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
    }


}

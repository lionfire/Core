using System.Collections.Generic;
#if AOT
using System.Collections;
#endif

namespace LionFire // RENAME LionFire.ExtensionMethods
{
    public static class HashSetExtensions
    {
        public static void TryAdd<T>(this HashSet<T> set, T val)
        {
            if (set.Contains(val)) return;
            set.Add(val);
        }
    }
}

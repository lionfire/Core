using System.Collections.Generic;

namespace LionFire.ExtensionMethods
{
    public static class ListExtensions
    {
        public static void Add<TK, TV>(this List<KeyValuePair<TK, TV>> list, TK key, TV val)
        {
            list.Add(new KeyValuePair<TK, TV>(key, val));
        }
    }
}

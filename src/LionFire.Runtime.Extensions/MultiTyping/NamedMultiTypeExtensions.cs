using LionFire.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    // ENH MEMORYLEAK: Clean up structures/data on removal (setting to null)

    public static class NamedMultiTypeExtensions
    {
        public static T AsType<T>(this IContainsMultiTyped cmt, string name)
        {
            var c = cmt.AsType<NamedMultiTypeContainer>();
            if (c == null) return default(T);
            return (T)c.Dictionary.TryGetValue(name);
        }
        public static T AsType<T>(this IContainsReadOnlyMultiTyped cmt, string name)
        {
            var c = cmt.AsType<NamedMultiTypeContainer>();
            if (c == null) return default(T);
            return (T)c.Dictionary.TryGetValue(name);
        }

        public static void SetType<T>(this IContainsMultiTyped cmt, string name, T value)
        {
            var c = cmt.AsType<NamedMultiTypeContainer>();
        }

        private class NamedMultiTypeContainer
        {
            public Dictionary<string, object> Dictionary = new Dictionary<string, object>();
        }
    }
}

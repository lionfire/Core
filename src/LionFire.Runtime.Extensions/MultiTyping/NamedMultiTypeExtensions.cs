using LionFire.ExtensionMethods;
using LionFire.MultiTyping;
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
        public static T AsTypeOrCreate<T>(this IContainsMultiTyped cmt, string name, Func<string, T> factory = null)
        {
            var c = cmt.AsTypeOrCreate<NamedMultiTypeContainer>();

            var result = c.AsType<T>(name);
            if (result == null)
            {
                result = factory != null ? factory(name) : (T)Activator.CreateInstance(typeof(T));
                c.SetType<T>(name, result);
            }
            return result;
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

            public T AsType<T>(string name)
            {
                if (Dictionary.ContainsKey(name)) return (T) Dictionary[name];
                return default(T);
            }
            public void SetType<T>(string name, T value)
            {
                if (Dictionary.ContainsKey(name)) Dictionary[name] = value;
                else Dictionary.Add(name, value);
            }
        }
    }
}

using LionFire.ExtensionMethods;
using System;
using System.Collections.Concurrent;

namespace LionFire.Structures
{
    public class NamedObjectsByType
    {
        ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> dict = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();

        public T Get<T>(string name = "") => (T)dict.TryGetValue(typeof(T))?.TryGetValue(name);
        public object Get(Type type, string name = "") => dict.TryGetValue(type)?.TryGetValue(name);

        public void Set<T>(T value, string name = "") => dict.GetOrAdd(typeof(T), t => new ConcurrentDictionary<string, object>()).AddOrUpdate(name, value, (a, b) => value);
        public bool TryAdd<T>(T value, string name = "") => dict.GetOrAdd(typeof(T), t => new ConcurrentDictionary<string, object>()).TryAdd(name, value);
        public bool TryRemove<T>(string name = "") => dict.TryGetValue(typeof(T))?.TryRemove(name, out object _) ?? false;
    }
}

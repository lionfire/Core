using System;
using System.Collections.Concurrent;

namespace LionFire.Referencing.Ex
{
    public static class HandleRegistry
    {
        public static ConcurrentDictionary<string, object> ReadHandles { get; } = new ConcurrentDictionary<string, object>();
        public static ConcurrentDictionary<string, object> WriteHandles { get; } = new ConcurrentDictionary<string, object>();
        public static ConcurrentDictionary<string, object> ReadWriteHandles { get; } = new ConcurrentDictionary<string, object>();

        //public static T TryGetReadWrite<T>(string key)
        //{
        //    if (ReadWriteHandles.TryGetValue(key, out var obj)) { return (T)obj; }
        //    return default;
        //}

        //public static T TryGetRead<T>(string key)
        //{
        //    if (ReadHandles.TryGetValue(key, out var obj)) { return (T)obj; }
        //    return default;
        //}

        //public static T TryGetWrite<T>(string key)
        //{
        //    if (WriteHandles.TryGetValue(key, out var obj)) { return (T)obj; }
        //    return default;
        //}

        public static T GetOrAddReadWrite<T>(string key, Func<string, object> factory) => (T)ReadWriteHandles.GetOrAdd(key, factory);
        public static T GetOrAddWrite<T>(string key, Func<string, object> factory) => (T)WriteHandles.GetOrAdd(key, factory);
        public static T GetOrAddRead<T>(string key, Func<string, object> factory) => (T)ReadHandles.GetOrAdd(key, factory);
    }
}

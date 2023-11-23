using System;
using System.Collections.Concurrent;
using LionFire.Dependencies;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Structures;

namespace LionFire.Referencing.Ex;

// static Replacement for HandleRegistry
public static class HandleRegistry2
{
    public static IHandleRegistry HandleRegistry
    {
        get
        {
            return DependencyContext.Current.GetService<IHandleRegistry>() ?? (LionFireEnvironment.IsMultiApplicationEnvironment ? throw new ArgumentNullException($"LionFireEnvironment.IsMultiApplicationEnvironment is true but there is no IHandleRegistry registered with {nameof(DependencyContext)}.Current") : ManualSingleton<WeakHandleRegistry>.GuaranteedInstance);
        }
    }

    public static string GetKeyWithType<T>(string key) => $"{typeof(T).FullName}){key}";
    public static IReadHandle<T> GetOrAddRead<T>(string key, Func<string, IReadHandle<T>> factory) => HandleRegistry.GetOrAddRead<T>(GetKeyWithType<T>(key), factory);
    public static IReadWriteHandle<T> GetOrAddReadWrite<T>(string key, Func<string, IReadWriteHandle<T>> factory) => HandleRegistry.GetOrAddReadWrite<T>(GetKeyWithType<T>(key), factory);
    public static IWriteHandle<T> GetOrAddWrite<T>(string key, Func<string, IWriteHandle<T>> factory) 
        => HandleRegistry.GetOrAddWrite<T>(GetKeyWithType<T>(key), factory);

    public static void AddOrUpdate<T>(string url, IReadHandle<T> handle)
        => HandleRegistry.AddOrUpdate<T>(url, handle);
}

public static class HandleRegistry
{
    public static ConcurrentDictionary<string, object> ReadHandles { get; } = new ConcurrentDictionary<string, object>();
    public static ConcurrentDictionary<string, object> ReadWriteHandles { get; } = new ConcurrentDictionary<string, object>();
    public static ConcurrentDictionary<string, object> WriteHandles { get; } = new ConcurrentDictionary<string, object>();

    //public static TValue TryGetReadWrite<TValue>(string key)
    //{
    //    if (ReadWriteHandles.TryGetValue(key, out var obj)) { return (TValue)obj; }
    //    return default;
    //}

    //public static TValue TryGetRead<TValue>(string key)
    //{
    //    if (ReadHandles.TryGetValue(key, out var obj)) { return (TValue)obj; }
    //    return default;
    //}

    //public static TValue TryGetWrite<TValue>(string key)
    //{
    //    if (WriteHandles.TryGetValue(key, out var obj)) { return (TValue)obj; }
    //    return default;
    //}

    // OPTIMIZE: shorter keys by 
    public static T GetOrAddRead<T>(string key, Func<string, object> factory) => (T)ReadHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
    public static T GetOrAddReadWrite<T>(string key, Func<string, object> factory) => (T)ReadWriteHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
    public static T GetOrAddWrite<T>(string key, Func<string, object> factory) => (T)WriteHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
}

public static class ObjectHandleRegistry
{
    public static ConcurrentDictionary<object, object> ReadHandles { get; } = new ConcurrentDictionary<object, object>();
    public static ConcurrentDictionary<object, object> WriteHandles { get; } = new ConcurrentDictionary<object, object>();
    public static ConcurrentDictionary<object, object> ReadWriteHandles { get; } = new ConcurrentDictionary<object, object>();

    public static T GetOrAddReadWrite<T>(string key, Func<object, object> factory) => (T)ReadWriteHandles.GetOrAdd(key, factory);
    public static T GetOrAddWrite<T>(string key, Func<object, object> factory) => (T)WriteHandles.GetOrAdd(key, factory);
    public static T GetOrAddRead<T>(string key, Func<object, object> factory) => (T)ReadHandles.GetOrAdd(key, factory);
}

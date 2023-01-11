using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace LionFire.Referencing.Ex;

// TODO NEXT
// - non-static dictionaries: use HandleCache class instead
// - add separated RW, W to HandleCache
// - consolidate with HandleCache: which should be WeakReferences, and which ones strong? All Weak?
// - Should I use Overby to attach HandleCache instance?  Idk how I'd do that.  IServiceProvider could be scoped.  Still, it might be nice to have a Context object() to act as a marker.  Maybe the IServiceProvider object is it.

public class HandleRegistrar
{
    #region Fields

    ConditionalWeakTable<string, IHandleBase> readHandles = new ConditionalWeakTable<string, IHandleBase>();
    ConditionalWeakTable<string, IHandleBase> readWriteHandles = new ConditionalWeakTable<string, IHandleBase>();
    ConditionalWeakTable<string, IHandleBase> writeHandles = new ConditionalWeakTable<string, IHandleBase>();

    #endregion
}

public interface IHandleRegistry
{
    T GetOrAddReadWrite<T>(string key, Func<string, object> factory);
    T GetOrAddWrite<T>(string key, Func<string, object> factory);
    T GetOrAddRead<T>(string key, Func<string, object> factory);
}

public static class HandleRegistry2<T>
{
    public static IHandleRegistry HandleRegistry
    {
        get
        {

        }
    }

    public static T GetOrAddReadWrite(string key, Func<string, object> factory) => (T)ReadWriteHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
    public static T GetOrAddWrite(string key, Func<string, object> factory) => (T)WriteHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
    public static T GetOrAddRead(string key, Func<string, object> factory) => (T)ReadHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);

}

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

    public static T GetOrAddReadWrite<T>(string key, Func<string, object> factory) => (T)ReadWriteHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
    public static T GetOrAddWrite<T>(string key, Func<string, object> factory) => (T)WriteHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
    public static T GetOrAddRead<T>(string key, Func<string, object> factory) => (T)ReadHandles.GetOrAdd(typeof(T).FullName + ":" + key, factory);
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

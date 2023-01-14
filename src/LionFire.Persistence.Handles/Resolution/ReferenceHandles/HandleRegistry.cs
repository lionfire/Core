﻿using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using BernhardHaus.Collections.WeakDictionary;
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

    public static IReadHandle<T> GetOrAddRead<T>(string key, Func<string, IReadHandle<T>> factory) => HandleRegistry.GetOrAddRead<T>(typeof(T).FullName + ":" + key, factory);
    public static IReadWriteHandle<T> GetOrAddReadWrite<T>(string key, Func<string, IReadWriteHandle<T>> factory) => HandleRegistry.GetOrAddReadWrite<T>(typeof(T).FullName + ":" + key, factory);
    public static IWriteHandle<T> GetOrAddWrite<T>(string key, Func<string, IWriteHandle<T>> factory) => HandleRegistry.GetOrAddWrite<T>(typeof(T).FullName + ":" + key, factory);

}

public static class HandleRegistry
{
    public static ConcurrentDictionary<string, object> ReadHandles { get; } = new ConcurrentDictionary<string, object>();
    public static ConcurrentDictionary<string, object> ReadWriteHandles { get; } = new ConcurrentDictionary<string, object>();
    public static ConcurrentDictionary<string, object> WriteHandles { get; } = new ConcurrentDictionary<string, object>();

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

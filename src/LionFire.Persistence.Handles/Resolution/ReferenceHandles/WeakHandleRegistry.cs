#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading;
using LionFire.Collections;
using LionFire.Collections.Concurrent;
using LionFire.Dependencies;
using LionFire.Persistence;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace LionFire.Persistence.Handles;

/// <summary>
/// Unused handles are GC'ed
/// </summary>
public class WeakHandleRegistry : IHandleRegistry, IDisposable
{
    public static void StaticDispose()
    {
        //#error NEXT: Is Up/Down Delta counters a better way?
        //#error NEXT: Is there a way to reset counters?
        //Meter.Dispose();
        //Meter = new("LionFire.Persistence.Handles.WeakHandleRegistry", "1.0");
    }
    private static ConcurrentHashSet<WeakHandleRegistry> instances = new();

    #region Metrics

    private static readonly Meter Meter = new("LionFire.Persistence.Handles.WeakHandleRegistry", "1.0");

    private static readonly Counter<long> ReadHandlesCreatedC = Meter.CreateCounter<long>("ReadHandlesCreated");

    ////private static readonly ObservableGauge<long> ReadHandlesC;// = Meter.CreateObservableGauge<long>("ReadHandles", () =>
    ////instances!.Select(i => i.readHandles == null ? 0 : i.readHandles.Count).Sum());
    ////private static readonly ObservableGauge<long> ReadHandlesC = Meter.CreateObservableGauge<long>("ReadHandles", () =>
    ////                                                           instances!.Select(i => i.readHandles == null ? 0 : i.readHandles.Count).Sum());

    //private readonly ObservableGauge<long> ReadHandlesC = Meter.CreateObservableGauge<long>("ReadHandles", () =>
    //(DependencyContext.Current?.GetService<IHandleRegistry>() as WeakHandleRegistry)?.readHandles?.Count ?? 0);
    private static readonly ObservableGauge<long> ReadHandlesC = Meter.CreateObservableGauge<long>("ReadHandles", () =>
                                                            ////(DependencyContext.Current?.GetService<IHandleRegistry>() as WeakHandleRegistry)?.readHandles?.Count ?? 0);
                                                            instances!.Select(i => i.readHandles == null ? 0 : i.readHandles.Count).Sum());

    #endregion

    #region Lifecycle

    static WeakHandleRegistry()
    {
        //ReadHandlesC = Meter.CreateObservableGauge<long>("ReadHandles", () =>
        //    instances!.Select(i => i.readHandles == null ? 0 : i.readHandles.Count).Sum());
    }
    public WeakHandleRegistry()
    {
        //ReadHandlesCreatedC = Meter.CreateCounter<long>("ReadHandlesCreated");
        //ReadHandlesC = Meter.CreateObservableGauge<long>("ReadHandles", () => readHandles?.Count ?? 0);
        instances.Add(this);
    }

    public void Dispose()
    {
        instances.Remove(this);
    }

    #endregion

    #region Fields

    private WeakDictionary<(string, Type), object> readHandles = new();
    //private WeakDictionary<string, object>? readWriteHandles;
    //private WeakDictionary<string, object>? writeHandles;

    #endregion

    private object _read = new();

    #region R

    public IReadHandle<TValue>? TryGetRead<TValue>(string uri)
    {
        if (readHandles == null) return null;

        var key = (uri, typeof(TValue));
        var value = readHandles[key];
        return value != null ? (IReadHandle<TValue>)value : null;
    }

    public IReadHandle<TValue> GetOrAddRead<TValue>(string uri, Func<string, IReadHandle<TValue>> factory)
    {
        var key = (uri, typeof(TValue));
        //if (readHandles.TryGetValue(key, out var found)) { return (IReadHandle<TValue>)found; }

        return (IReadHandle<TValue>)readHandles.GetOrAdd(key, _ => factory(uri));

        //lock (_read)
        //{
        //    if (readHandles.TryGetValue(key, out found)) { return (IReadHandle<TValue>)found; }
        //    if (readHandles.TryGetValue(key, out found)) { return (IReadHandle<TValue>)found; }

        //    var value = factory(uri);
        //    try
        //    {
        //        readHandles.Add(key, value);
        //    }
        //    catch (ArgumentException ex) when (ex.Message.Contains("already"))
        //    {
        //        if (readHandles.TryGetValue(key, out found)) { return (IReadHandle<TValue>)found; }
        //        else throw new Exception("Already exists, but can't get value");
        //    }
        //    ReadHandlesCreatedC.IncrementWithContext();
        //    return value;
        //}
    }

    #endregion

    #region RW

    public IReadWriteHandle<TValue> TryGetReadWrite<TValue>(string uri)
    {
        throw new NotImplementedException();
    }
    public IReadWriteHandle<TValue> GetOrAddReadWrite<TValue>(string uri, Func<string, IReadWriteHandle<TValue>> factory)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region W

    public IWriteHandle<TValue> TryGetWrite<TValue>(string uri)
    {
        throw new NotImplementedException();
    }
    public IWriteHandle<TValue> GetOrAddWrite<TValue>(string uri, Func<string, IWriteHandle<TValue>> factory)
    {
        throw new NotImplementedException();
    }

    #endregion



}

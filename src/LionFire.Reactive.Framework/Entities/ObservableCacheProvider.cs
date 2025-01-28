using LionFire.Reactive.Persistence;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace LionFire.Reactive.Entities;

// REVIEW - rethink and simplify
public class ObservableCacheProvider
{
    public IServiceProvider ServiceProvider { get; }

    public ObservableCacheProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }


    #region Convenience


    public IObservableReader<TKey, TValue> GetReader<TKey, TValue>(object? serviceKey = null)
        where TKey : notnull
        where TValue : notnull
    {
        var result = ServiceProvider.GetRequiredKeyedService<IObservableReaderWriterComponents<TKey, TValue>>(serviceKey);
        return result.Read;
    }
    public IObservableWriter<TKey, TValue> GetWriter<TKey, TValue>(object? serviceKey = null)
            where TKey : notnull
            where TValue : notnull
    {
        var result = ServiceProvider.GetRequiredKeyedService<IObservableReaderWriterComponents<TKey, TValue>>(serviceKey);
        return result.Write;
    }

    #endregion


    /// <summary>
    /// Looks up from IServiceProvider.  Fallback: combines Reader and Writer if available
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="serviceKey"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IObservableReaderWriter<TKey, TValue> GetReaderWriter<TKey, TValue>(object? serviceKey = null)
        where TKey : notnull
        where TValue : notnull
    {
        var rw = ServiceProvider.GetKeyedService<IObservableReaderWriter<TKey, TValue>>(serviceKey);
        if (rw != null) return rw;
        var rwc = ServiceProvider.GetKeyedService<IObservableReaderWriterComponents<TKey, TValue>>(serviceKey);
        if (rwc is IObservableReaderWriter<TKey, TValue> orw) return orw;
        else if(rwc != null) return new ObservableReaderWriterFromComponents<TKey, TValue>(rwc.Read, rwc.Write);

        var r = ServiceProvider.GetKeyedService<IObservableReader<TKey, TValue>>(serviceKey);
        var w = ServiceProvider.GetKeyedService<IObservableWriter<TKey, TValue>>(serviceKey);
        if (r == null || w == null) throw new Exception("No IObservableReaderWriter found, and no IObservableReader and IObservableWriter found.");
        return new ObservableReaderWriterFromComponents<TKey, TValue>(r, w);
    }
}

// OLD
//public class InMemoryWorkspaceProvider : IWorkspaceProvider
//{
//    private ConcurrentDictionary<string, IWorkspace> workspaces = new ConcurrentDictionary<string, IWorkspace>();

//    public IWorkspace Create(string key, IReference? template = null) => workspaces.GetOrAdd(key, k => new Workspace()); // RENAME to QueryOrCreate?
//    public IWorkspace? Query(string key)
//    {
//        return workspaces.TryGetValue(key);
//    }

//    //public IWorkspace GetOrCreate(string key) // TODO
//    //{

//    //}
//}

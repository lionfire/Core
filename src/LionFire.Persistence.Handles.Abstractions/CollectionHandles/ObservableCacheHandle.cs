#if TODO
using DynamicData;
using DynamicData.Kernel;
using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles;

public class ObservableCacheHandle<TValue> : IObservableCache<TValue, string>
    where TValue : notnull
{
    #region Parameters

    public IReadHandle<Metadata<IEnumerable<IListing<TValue>>>> Upstream { get; set; }

    #endregion
    
    #region State

    SourceCache<TValue, string> cache = new SourceCache<TValue, string>(x => x.Key);

    #endregion
    public Optional<TValue> Lookup(string key)
    {
        return cache.Lookup(key);
    }

    public IObservable<IChangeSet<TValue, string>> Connect(Func<TValue, bool>? predicate = null, bool suppressEmptyChangeSets = true)
    {
        return cache.Connect(predicate, suppressEmptyChangeSets);
    }

    public IObservable<IChangeSet<TValue, string>> Preview(Func<TValue, bool>? predicate = null)
    {
        return cache.Preview(predicate);
    }

    public IObservable<Change<TValue, string>> Watch(string key)
    {
        return cache.Watch(key);
    }

    public void Dispose()
    {
        cache.Dispose();
    }

}
#endif
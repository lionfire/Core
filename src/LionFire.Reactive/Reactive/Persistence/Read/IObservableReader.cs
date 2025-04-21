using DynamicData.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Reactive.Persistence;

//public interface IObservableData<TKey, TValue> 
//    where TKey : notnull
//    where TValue : notnull
//{
//}

public interface IObservableReader<TKey, TValue> : IObservableReader<TKey, TValue, TKey>
    where TKey : notnull
    where TValue : notnull
;

// Thoughts: Listen could be a separate interface
public interface IObservableReader<TKey, TValue, TMetadata>
    where TKey : notnull
    where TValue : notnull
    where TMetadata : notnull
{

    #region Keys

    IObservableCache<TMetadata, TKey> Keys { get; }

    // ENH - Return a task that completes after the initial loading of missing values is complete (like ListenAllValues)
    IDisposable ListenAllKeys();  // ENH - add CancelationToken parameter for initial load

    #endregion

    #region Values

    // REVIEW - Values redundant with ObservableCache?

    /// <summary>
    /// Only subscribe if you want to load (deserialize) all available items
    /// </summary>
    IObservableCache<Optional<TValue>, TKey> Values { get; }

    /// <summary>
    /// </summary>
    /// <returns>
    /// Returns a task that completes after the initial loading of missing values is complete.  TODO: Also wait for initial loading of Keys
    /// </returns>
    ValueTask<IDisposable> ListenAllValues(); // ENH - add CancelationToken parameter for initial load
    IObservableCache<Optional<TValue>, TKey> ObservableCache { get; }

    //ValueTask<Optional<TValue>> TryGetValue(TKey key);
    IObservable<TValue?>? GetValueObservableIfExists(TKey key);
    IObservable<TValue?> GetValueObservable(TKey key);


    #endregion

    /// <summary>
    /// Listen to specified keys (perhaps because they are in view)
    /// </summary>
    //SourceList<TKey> ListenTo { get; set; }
}

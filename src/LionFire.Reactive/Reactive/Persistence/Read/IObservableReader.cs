using DynamicData.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Reactive.Persistence;

// Thoughts: Listen could be a separate interface

public interface IObservableReader<TKey, TValue> : IObservableReader<TKey, TValue, TKey>
    where TKey : notnull
    where TValue : notnull
;

public interface IObservableReader<TKey, TValue, TMetadata>
    where TKey : notnull
    where TValue : notnull
    where TMetadata : notnull
{

    IObservableCache<TMetadata, TKey> Keys { get; }

    /// <summary>
    /// Only subscribe if you want to load (deserialize) all available items
    /// </summary>
    IObservableCache<Optional<TValue>, TKey> Values { get; }

    IObservable<TValue?>? GetValueObservableIfExists(TKey key);
    IObservable<TValue?> GetValueObservable(TKey key);

    IDisposable ListenAll();

    /// <summary>
    /// Listen to specified keys (perhaps because they are in view)
    /// </summary>
    //SourceList<TKey> ListenTo { get; set; }
}

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
    IObservableCache<TValue, TKey> ObservableCache { get; }

    IObservable<TValue?> Listen(TKey key);

}

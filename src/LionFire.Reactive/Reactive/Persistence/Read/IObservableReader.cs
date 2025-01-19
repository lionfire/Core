using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Reactive.Persistence;

// Thoughts: Listen could be a separate interface

public interface IObservableReader<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{

    IObservableList<TKey> Keys { get; }

    /// <summary>
    /// Only subscribe if you want to load (deserialize) all available items
    /// </summary>
    IObservableCache<TValue, TKey> Items { get; }

    IObservable<TValue?> Listen(TKey key);

}

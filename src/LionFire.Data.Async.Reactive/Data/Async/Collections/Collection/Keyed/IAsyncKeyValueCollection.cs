using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Collections;

public interface IAsyncKeyValueCollection<TKey, TItem> : IAsyncCollection<KeyValuePair<TKey, TItem>>
    where TKey : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; }

    ValueTask<bool> Remove(TKey key);
}

using DynamicData;
using Microsoft.Extensions.Logging;

namespace LionFire.Orleans_.Collections;

public class InMemoryKeyedCollectionG<TKey, TItem> : KeyedCollectionGBase<TKey, TItem>
    //, IAsyncCreating<TNotificationItem> 
    //, ICreatingAsyncDictionary<string, TNotificationItem>
    where TKey : notnull
    where TItem : notnull
{
    public InMemoryKeyedCollectionG(IServiceProvider serviceProvider, ILogger<InMemoryKeyedCollectionG<TKey, TItem>> logger) : base(serviceProvider, logger)
    {
    }

    #region State

    //protected override IObservableCache<TValue, TKey> ItemsDictionary => items;

    #endregion
}

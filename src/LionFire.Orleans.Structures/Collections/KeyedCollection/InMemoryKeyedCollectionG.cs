using Microsoft.Extensions.Logging;

namespace LionFire.Orleans_.Collections;

public class InMemoryKeyedCollectionG<TKey, TItem> : KeyedCollectionGBase<TKey, TItem>
    //, IAsyncCreating<TNotificationItem> 
    //, ICreatingAsyncDictionary<string, TNotificationItem>
    where TKey : notnull
    where TItem : notnull
{
    public InMemoryKeyedCollectionG(IServiceProvider serviceProvider, /* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */  ILogger<InMemoryKeyedCollectionG<TKey, TItem>> logger) : base(serviceProvider, logger)
    {
        //Items = new();
    }

    #region State

    protected override IDictionary<TKey, TItem> ItemsDictionary  => items;
    protected Dictionary<TKey, TItem> items { get; } = new();

    #endregion
}

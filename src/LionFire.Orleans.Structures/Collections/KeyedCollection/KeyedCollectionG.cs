
using LionFire.Collections;
using LionFire.Orleans_.Reactive_;
using LionFire.Data.Async.Gets;
using LionFire.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using static LionFire.Reflection.GetMethodEx;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DynamicData;
using Orleans.Streams;
using LionFire.Orleans_.ObserverGrains;

namespace LionFire.Orleans_.Collections;

/// <summary>
/// Implemented with Dictionary (so keys must be unique) but you add/remove items, and a Func<TValue, TKey> is used to extract the key from the item.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class KeyedCollectionG<TKey, TValue> : KeyedCollectionGBase<TKey, TValue> // RENAME KeyableDictionaryG
//, IAsyncCreating<TNotificationItem> 
//, ICreatingAsyncDictionary<string, TNotificationItem>
where TKey : notnull
where TValue : notnull
{

    #region Lifecycle

    public KeyedCollectionG(IServiceProvider serviceProvider, /*[PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)]*/ IPersistentState<Dictionary<TKey, TValue>> items, ILogger<KeyedCollectionG<TKey, TValue>> logger) : base(serviceProvider, logger)
    {
        ItemsState = items;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        ItemsState.State ??= new();
        return base.OnActivateAsync(cancellationToken);
    }

    #endregion

    #region State

    //protected override IObservableCache<TValue, TKey> ItemsDictionary  => ItemsState.State;
    protected IPersistentState<Dictionary<TKey, TValue>> ItemsState { get; }

    #endregion

    #region Methods

    protected override async ValueTask OnStateHasChanged() => await ItemsState.WriteStateAsync();


    #endregion
}

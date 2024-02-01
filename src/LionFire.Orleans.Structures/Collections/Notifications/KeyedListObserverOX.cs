using DynamicData;
using LionFire.ExtensionMethods;
using LionFire.Ontology;
using LionFire.Orleans_.ObserverGrains;
using System.Reactive.Linq;

namespace LionFire.Orleans_.Collections;

public static class KeyedListObserverOX
{
    public static async Task<IAsyncDisposable> SubscribeViaGrainObserver<TKey, TItem>(this IAsyncObserver<ChangeSet<TItem, TKey>> observer, IClusterClient clusterClient, IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>>? grain = null)
        where TKey : notnull
    {
        var subscribes = observer as ISubscribesAsync;
        if (subscribes?.Subscriptions.OfType<KeyedListObserverO<TKey, TItem>>().Any() == true) return System.Reactive.Disposables.AsyncDisposable.Nop;

        grain = (observer as IHas<IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>>>)?.Object;
        if (grain == null)
        {
            throw new ArgumentException($"Must provide grain, or implement {nameof(IHas<IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>>>)} on the {nameof(observer)}");
        }
        var keyedListObserverO = new KeyedListObserverO<TKey, TItem>(grain, clusterClient);
        subscribes?.OnSubscribing(keyedListObserverO);
        await keyedListObserverO.SubscribeAsync(observer).ConfigureAwait(false);
        return keyedListObserverO;
    }

    public static bool IsSubscribedViaGrainObserver<TKey, TItem>(this IAsyncObserver<ChangeSet<TItem, TKey>> observer, IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>>? grain = null)
    {
        var subscribes = observer as ISubscribesAsync;

        return (subscribes?.Subscriptions.OfType<KeyedListObserverO<TKey, TItem>>().Any() == true);
    }


}

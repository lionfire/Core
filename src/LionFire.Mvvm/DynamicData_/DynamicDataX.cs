using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using System;
using System.Reactive.Linq;

namespace LionFire.ExtensionMethods;

public static class DynamicDataX
{
    public static IObservable<IChangeSet<TValue, TKey>> BindToObservableListAction<TValue, TKey>(this IObservable<IChangeSet<TValue, TKey>> observable, Action<IObservableList<TValue>?> onList)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(observable);
        DynamicData.IObservableList<TValue>? list;
        IObservable<IChangeSet<TValue, TKey>> returnValue = observable;
        if (observable == null)
        {
            list = null;
        }
        else
        {
            returnValue = observable.BindToObservableList(out list);
        }
        onList(list);
        return returnValue!;
    }

    /// <summary>
    /// Transform items in the observable stream, but update existing items in-place when the source item changes.
    /// This is useful for lazy-loading scenarios where VMs are created initially with null values,
    /// then updated when the actual value loads.
    /// </summary>
    /// <remarks>
    /// This is a custom implementation because DynamicData's built-in TransformWithInlineUpdate requires
    /// the updateAction parameter, whereas this version makes it optional. When updateAction is null,
    /// updates create a new transformed instance (standard Transform behavior). This allows the same
    /// operator to be used for both standard transforms and inline-update transforms.
    /// </remarks>
    /// <typeparam name="TObject">Source object type</typeparam>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TDestination">Destination object type</typeparam>
    /// <param name="source">Source observable</param>
    /// <param name="transformFactory">Factory to create new destination objects</param>
    /// <param name="updateAction">Action to update existing destination object when source changes (can be null to skip updates)</param>
    /// <returns>Observable of transformed items</returns>
    public static IObservable<IChangeSet<TDestination, TKey>> TransformWithInlineUpdate<TObject, TKey, TDestination>(
        this IObservable<IChangeSet<TObject, TKey>> source,
        Func<TObject, TKey, TDestination> transformFactory,
        Action<TDestination, TObject>? updateAction = null)
        where TKey : notnull
        where TDestination : notnull
    {
        return source.Scan((ChangeAwareCache<TDestination, TKey>?)null, (cache, changes) =>
        {
            cache ??= new ChangeAwareCache<TDestination, TKey>(changes.Count);

            foreach (var change in changes)
            {
                switch (change.Reason)
                {
                    case ChangeReason.Add:
                        cache.AddOrUpdate(transformFactory(change.Current, change.Key), change.Key);
                        break;
                    case ChangeReason.Update:
                        {
                            if (updateAction == null)
                            {
                                // No update action - replace with new instance
                                cache.AddOrUpdate(transformFactory(change.Current, change.Key), change.Key);
                            }
                            else
                            {
                                var previous = cache.Lookup(change.Key);
                                if (previous.HasValue)
                                {
                                    updateAction(previous.Value, change.Current);
                                    cache.Refresh(change.Key);
                                }
                                else
                                {
                                    // Key not found, add new
                                    cache.AddOrUpdate(transformFactory(change.Current, change.Key), change.Key);
                                }
                            }
                        }
                        break;
                    case ChangeReason.Remove:
                        cache.Remove(change.Key);
                        break;
                    case ChangeReason.Refresh:
                        cache.Refresh(change.Key);
                        break;
                    case ChangeReason.Moved:
                        // No action needed for moved items
                        break;
                }
            }
            return cache;
        }).Select(cache => cache!.CaptureChanges());
    }
}

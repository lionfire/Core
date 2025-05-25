using ReactiveUI;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;

namespace LionFire.Reactive.Persistence;

public static class ObservableDataReadWriteX
{

    #region Autosave

    //public static IDisposable AutosaveValueChanges<TKey, TValue>(this IObservableReaderWriterComponents<TKey, TValue> components)
    //    where TKey : notnull
    //    where TValue : notnull
    //    => AutosaveValueChangesFromComponents(components.Read, components.Write);
    public static IDisposable AutosaveValueChanges<TKey, TValue>(this IObservableReaderWriter<TKey, TValue> components)
    where TKey : notnull
    where TValue : notnull
    => AutosaveValueChanges(components, components);

    public static IDisposable AutosaveValueChanges<TKey, TValue>(this IObservableReader<TKey, TValue> reader, IObservableWriter<TKey, TValue> writer)
        where TKey : notnull
        where TValue : notnull
    {
        ConcurrentDictionary<TKey, IDisposable> subscriptions = new();

        var subscription = reader.Values.Connect().Subscribe(changes =>
        {
#if false // If I can get reader.Values to emit Updates, I can use this simpler code block
            foreach (var change in changes.Where(c=>c.Reason == ChangeReason.Update && c.Current.HasValue))
            {
                Task.Run(async () => await writer.Write(change.Key, change.Current.Value));
                //await writer.Write(change.Key, change.Current.Value);

                    //var v = change.Current.Value;

                    //if (typeof(TValue).IsAssignableTo(typeof(IReactiveNotifyPropertyChanged<IReactiveObject>)))
                    //{
                    //    var reactiveValue = (IReactiveNotifyPropertyChanged<IReactiveObject>)v;

                    //    var valueChangesSubscription = reactiveValue.Changed.Subscribe(e =>
                    //    {
                    //        Task.Run(async () => await writer.Write(change.Key, v));
                    //    });
                    //    subscriptions.AddOrUpdate(change.Key, valueChangesSubscription, (k, _) => valueChangesSubscription);
                    //}
                    ////else if (v is INotifyPropertyChanged inpc)
                    ////{
                    ////    inpc.PropertyChanged +=
                    ////}
                    //else
                    //{
                    //    Debug.WriteLine($"[ObservableDataReadWriteX Autosave] Type not supported: {typeof(TValue)}");
                    //    //throw new NotSupportedException();
                    //}
            }
#else
            foreach (var change in changes)
            {
                //DynamicData.Kernel.Optional<TValue> x;
                //if (ReferenceEquals(change.Current, change.Previous))
                if (/*(change.Current == null && change.Previous == null) ||*/ change.Current.Equals(change.Previous))
                {
                    Debug.WriteLine($"Ignoring: {change}");
                    continue;
                }
                if (subscriptions.TryRemove(change.Key, out var d))
                {
                    try
                    {
                        // REVIEW - why is this being reached twice for each change?
                        // ENH - when deserializing, preserve existing object instance to avoid reattaching
                        d.Dispose(); // TEMP comment
                    }
                    catch { } // EMPTYCATCH
                }

                if (change.Current.HasValue)
                {
                    var v = change.Current.Value;

                    if (typeof(TValue).IsAssignableTo(typeof(IReactiveNotifyPropertyChanged<IReactiveObject>)))
                    {
                        var reactiveValue = (IReactiveNotifyPropertyChanged<IReactiveObject>)v;

                        var valueChangesSubscription = reactiveValue.Changed.Subscribe(e =>
                        {
                            Task.Run(async () => await writer.Write(change.Key, v));
                        });
                        subscriptions.AddOrUpdate(change.Key, valueChangesSubscription, (k, _) => valueChangesSubscription);
                    }
                    //else if (v is INotifyPropertyChanged inpc)
                    //{
                    //    inpc.PropertyChanged +=
                    //}
                    else
                    {
                        Debug.WriteLine($"[ObservableDataReadWriteX Autosave] Type not supported: {typeof(TValue)}");
                        //throw new NotSupportedException();
                    }
                }
            }
#endif
        });

        return Disposable.Create(() =>
        {
            subscription.Dispose();
            foreach (var kvp in subscriptions)
            {
                kvp.Value.Dispose();
            }
            subscriptions.Clear();
        });
    }

    #endregion
}

using DynamicData.Binding;
using System;

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
}

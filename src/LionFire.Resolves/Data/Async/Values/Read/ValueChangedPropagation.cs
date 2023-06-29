using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LionFire.Data.Gets;


/// <summary>
/// Listen to objects that implement INotifyPropertyChanged and propagate these events
/// </summary>
/// <remarks>
/// Owner only has one attached at a time
/// </remarks>
public class ValueChangedPropagation
{
    object tabLock = new object();
    ConditionalWeakTable<object, PropertyChangedEventHandler> tab = new ConditionalWeakTable<object, PropertyChangedEventHandler>();

    public void Attach(object owner, object? o, Action<object> fire)
    {
        if (o is INotifyPropertyChanged inpc)
        {
            lock (tabLock)
            {
                _detach(owner, inpc);
                var x = new PropertyChangedEventHandler((sender, evt) => fire(sender));
                inpc.PropertyChanged += x;
                tab.Add(owner, x);
            }
        }
    }
    private void _detach(object owner, INotifyPropertyChanged inpc)
    {
        if (tab.TryGetValue(owner, out var existing))
        {
            inpc.PropertyChanged -= existing;
            tab.Remove(owner);
        }
    }
    public void Detach(object owner, object? o)
    {
        if (o == null) return;
        if (o is INotifyPropertyChanged inpc)
        {
            lock (tabLock)
            {
                _detach(owner, inpc);
            }
        }
    }
}


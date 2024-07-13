using ReactiveUI;
using System.Reactive.Subjects;

namespace LionFire.Inspection.Nodes;

public class ReactiveNotifyPropertyChanged<T> : IReactiveNotifyPropertyChanged<T>
{
    public void RaisePropertyChanging(T sender, string propertyName)
    {
        if (suppress <= 0) changing.OnNext(new ReactivePropertyChangingEventArgs<T>(sender, propertyName));
    }
    public void RaisePropertyChanged(T sender, string propertyName)
    {
        if (suppress <= 0) changed.OnNext(new ReactivePropertyChangedEventArgs<T>(sender, propertyName));
    }

    public IObservable<IReactivePropertyChangedEventArgs<T>> Changing => changing;
    Subject<IReactivePropertyChangedEventArgs<T>> changing = new();

    public IObservable<IReactivePropertyChangedEventArgs<T>> Changed => changed;
    Subject<IReactivePropertyChangedEventArgs<T>> changed = new();

    #region Suppress

    public IDisposable SuppressChangeNotifications()
    {
        suppress++;
        return new Suppressor(this);
    }

    int suppress = 0;

    internal class Suppressor : IDisposable
    {
        private ReactiveNotifyPropertyChanged<T>? sourceChangeEvents;

        internal Suppressor(ReactiveNotifyPropertyChanged<T> sourceChangeEvents)
        {
            this.sourceChangeEvents = sourceChangeEvents;
        }

        public void Dispose() { (sourceChangeEvents ?? throw new ObjectDisposedException("")).suppress = Math.Max(0, sourceChangeEvents.suppress - 1); sourceChangeEvents = null; }
    }

    #endregion
}

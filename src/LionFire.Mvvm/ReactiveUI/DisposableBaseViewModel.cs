using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ReactiveUI_;


// This class by @AmirMahdiNassiri Retrieved from https://github.com/reactiveui/ReactiveUI/issues/2932#issuecomment-919355298

public class DisposableBaseViewModel : ReactiveObject, IDisposable
{
    protected CompositeDisposable Disposables { get; }
    protected Dictionary<string, IDisposable> NestedViewModelSubscriptions { get; }

    public DisposableBaseViewModel()
    {
        Disposables = new CompositeDisposable();
        NestedViewModelSubscriptions = new Dictionary<string, IDisposable>();
    }

    protected T? RaiseAndSetNestedViewModelIfChanged<T>(
        ref T nestedViewModel,
        T newValue,
        [CallerMemberName] string propertyName = null)
        where T : IReactiveObject
    {
        if (EqualityComparer<T>.Default.Equals(nestedViewModel, newValue))
        {
            return newValue;
        }

        if (NestedViewModelSubscriptions.TryGetValue(propertyName, out var oldSubscription))
        {
            if (!Disposables.Remove(oldSubscription))
                oldSubscription.Dispose();

            NestedViewModelSubscriptions.Remove(propertyName);
        }

        var returnValue = this.RaiseAndSetIfChanged(ref nestedViewModel, newValue, propertyName);

        if (returnValue != null)
        {
            var subscriptionHandle = Observable.FromEvent<PropertyChangedEventHandler, Unit>(
                eventHandler =>
                {
                    void Handler(object sender, PropertyChangedEventArgs e) => eventHandler(Unit.Default);
                    return Handler!;
                },
                eh => returnValue.PropertyChanged += eh,
                eh => returnValue.PropertyChanged -= eh)
                .Subscribe(_ => this.RaisePropertyChanged(propertyName))
                .DisposeWith(Disposables);

            NestedViewModelSubscriptions[propertyName] = subscriptionHandle;
        }

        return returnValue;
    }

    public void Dispose()
    {
        Disposables.Dispose();
    }
}




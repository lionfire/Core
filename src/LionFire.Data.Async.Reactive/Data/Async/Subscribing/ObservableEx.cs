﻿using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive;
using ReactiveUI;

namespace LionFire.Subscribing;

public static class ObservableEx2 // REVIEW - class name. Conflicts with Microsoft. 
{
    // REVIEW - isn't this in a 3rd party library?
    public static IObservable<T> Return<T>(T value) => Observable.Create<T>((Func<IObserver<T>, IDisposable>)(o =>
    {
        o.OnNext(value);
        o.OnCompleted();
        return Disposable.Empty;
    }));
}

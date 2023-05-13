using DynamicData;
using LionFire.Reactive;
using LionFire.ReactiveUI_;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Subscribing;

public interface ISubscribes<T>
{
    IObservable<ChangeSet<T>> Changes { get; }

    ValueTask Subscribe();
    ValueTask Unsubscribe();
    bool IsSubscribed { get; set; }
}

///// <typeparam name="TSubscription">(If there is one subscription per subscriber, you can also think of this as TSubscriber.)</typeparam>
//public interface IPublishes<TSubscription>
//{
//    ValueTask Unsubscribe(TSubscription subscription);
//}

public static class SubscribeCommands
{
    public static ReactiveCommand<bool, Task<bool>> CreateSubscribeCommand<T>(this ISubscribes<T> s) => s == null ? CannotExecuteReactiveCommand<bool, Task<bool>>.Instance : ReactiveCommand.Create<bool, Task<bool>>(async b =>
           {
               if (b)
               {
                   await s.Subscribe().ConfigureAwait(false);
               }
               else
               {
                   await s.Unsubscribe().ConfigureAwait(false);
               }
               return b;
           });

    //public bool CanSubscribe => Items is IAsyncObservable<NotifyCollectionChangedEventArgs<TValue>>;
    //public bool IsSubscribed => sub != null;
    //private bool IsSubscribing;

    //private IAsyncObservableForSyncObservers<Collections.NotifyCollectionChangedEventArgs<TKey>>? Subscribable => Items as IAsyncObservableForSyncObservers<Collections.NotifyCollectionChangedEventArgs<TKey>>;
    //Guid G = Guid.NewGuid();

    // OLD - ReactiveCommand takes care of this
    //public async Task Subscribe()
    //{
    //    var subscribable = Subscribable;
    //    if (subscribable == null)
    //    {
    //        return;
    //        //throw new NotSupportedException(); 
    //    }
    //    if (!IsSubscribing)
    //    {
    //        try
    //        {
    //            IsSubscribing = true;
    //            if (sub != null) return;
    //            Debug.WriteLine($"Subscribe {G}");
    //            throw new NotImplementedException();
    //            //sub = await subscribable.Subscribe(this).ConfigureAwait(false);
    //        }
    //        finally
    //        {
    //            IsSubscribing = false;
    //        }
    //    }
    //}

    //public ValueTask Unsubscribe()
    //{
    //    var subCopy = sub;
    //    if (subCopy != null)
    //    {
    //        sub = null;
    //        return subCopy.DisposeAsync();
    //    }
    //    else { return ValueTask.CompletedTask; }
    //}

    //IAsyncDisposable? sub
    //{
    //    get => _sub;
    //    set
    //    {
    //        if (ReferenceEquals(_sub, value)) return;
    //        if (_sub != null && value != null) { throw new AlreadySetException(); }
    //        _sub = value;
    //    }
    //}
    //IAsyncDisposable? _sub;

}

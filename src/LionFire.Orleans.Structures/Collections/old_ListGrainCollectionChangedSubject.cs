//using Orleans.Streams;
//using System.Reactive.Subjects;

//namespace LionFire.Orleans_.Collections;

//internal class ListGrainCollectionChangedSubject<TValue> : ISubject<ListGrainCollectionChangedEvent<TValue>>
//{
//    private readonly IClusterClient clusterClient;
//    private IListG<TValue> listGrain;

//    public ListGrainCollectionChangedSubject(IClusterClient clusterClient, IListG<TValue> listGrain)
//    {
//        this.clusterClient = clusterClient;
//        this.listGrain = listGrain;
//    }

//    public Subject<ListGrainCollectionChangedEvent<TValue>> Subject { get; } = new();

//    #region IObserver
    
//    public void OnCompleted()
//    {
//        ((IObserver<ListGrainCollectionChangedEvent<TValue>>)Subject).OnCompleted();
//    }

//    public void OnError(Exception error)
//    {
//        ((IObserver<ListGrainCollectionChangedEvent<TValue>>)Subject).OnError(error);
//    }

//    public void OnNext(ListGrainCollectionChangedEvent<TValue> value)
//    {
//        ((IObserver<ListGrainCollectionChangedEvent<TValue>>)Subject).OnNext(value);
//    }

//    #endregion

//    #region IObservable

//    public IDisposable Connect(IObserver<ListGrainCollectionChangedEvent<TValue>> observer)
//    {
//        if(!Subject.HasObservers)
//        {
//            SubscribeToStream();
//        }
//        var inner = ((IObservable<ListGrainCollectionChangedEvent<TValue>>)Subject).Connect(observer);
//        return new ListGrainCollectionChangedDisposableWrapper<TValue>(this, inner);
//    }

//    #endregion

//    private class ListGrainCollectionChangedDisposableWrapper<TValue> : IDisposable
//    {
//        private IDisposable inner;
//        private ListGrainCollectionChangedSubject<TValue> subject;


//        public ListGrainCollectionChangedDisposableWrapper(ListGrainCollectionChangedSubject<TValue> listGrainCollectionChangedSubject, IDisposable inner)
//        {
//            this.subject = listGrainCollectionChangedSubject;
//            this.inner = inner;
//        }

//        public void Dispose()
//        {
//            inner.Dispose();
//            subject.OnSubscriberDisposed();
//        }
//    }

//    private void OnSubscriberDisposed()
//    {
//        if (!Subject.HasObservers)
//        {
//            UnsubscribeFromStream();
//        }
//    }

//    public async Task SubscribeToStream()
//{
//        //IClusterClient c;
//        ////c.GetStreamProvider;
//        //IGrainFactory f;
//        //IGrain g;
//        var handle = await clusterClient.GetStreamProvider("s").GetStream<ListGrainCollectionChangedEvent<TValue>>(Guid.Empty,"asdf").Connect(Subject).ConfigureAwait(false);
//    }
//    public void UnsubscribeFromStream()
//    {

//    }

//}

////public static class IListGrainExtensions
////{
////    public static IDisposable Connect(this IListG observer)
////    {
////        IListGrainExtensionsData<TValue>.ListGrainCollectionChangedSubjects[observer].Connect(observer);

////        //IObserver<ListGrainCollectionChangedEvent<TValue>>

////    }
////    //Subject<> CollectionChangedSubject { get; } = new();
////    //void X()
////    //{
////    //    CollectionChangedSubject.
////    //}

////}



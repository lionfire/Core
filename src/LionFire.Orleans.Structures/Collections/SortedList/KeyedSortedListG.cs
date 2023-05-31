namespace LionFire.Orleans_.Collections;

public class KeyedSortedListG<TKey, TItem>
{
    public KeyedSortedListG()
    {
        throw new NotImplementedException("Consider KeyedCollectionG");
    }
}

#if Triage
//public static class IListGrainExtensionsData<TNotificationItem>
//{
//    static ConcurrentWeakDictionaryCache<IListG<TNotificationItem>, ListGrainCollectionChangedSubject<TNotificationItem>> ListGrainCollectionChangedSubjects { get; } = new(listGrain => new ListGrainCollectionChangedSubject<TNotificationItem>(listGrain));
//}

//public static class IListGrainExtensions
//{
//    public static IDisposable Subscribe(this IListG subscriber)
//    {
//        IListGrainExtensionsData<TNotificationItem>.ListGrainCollectionChangedSubjects[subscriber].Subscribe(subscriber);

//        //IObserver<ListGrainCollectionChangedEvent<TNotificationItem>>

//    }
//    //Subject<> CollectionChangedSubject { get; } = new();
//    //void X()
//    //{
//    //    CollectionChangedSubject.
//    //}

//}

#endif
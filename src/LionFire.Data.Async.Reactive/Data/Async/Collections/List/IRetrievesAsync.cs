namespace LionFire.Data.Async.Collections;


// I want some of this but I wonder if I can get it from IObservableGets and other interfaces
// Superfluous to IObservableGets<TItem>
//public interface IAsyncRetrieves<out TItem> : IReadOnlyCollection<TItem>, INotifyPropertyChanged
//{
//    bool HasRetrieved { get; }
//    bool IsRetrieving { get; } // Change type to IObservable<bool>
//    bool CanRetrieve { get; }
//    ITask<IEnumerable<TItem>> Retrieve(bool syncToCollection = true, CancellationToken cancellationToken = default);
//}
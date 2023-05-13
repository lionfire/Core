namespace LionFire.Collections.Async;


// I want some of this but I wonder if I can get it from IObservableResolves and other interfaces
// Superfluous to IObservableResolves<TItem>
//public interface IAsyncRetrieves<out TItem> : IReadOnlyCollection<TItem>, INotifyPropertyChanged
//{
//    bool HasRetrieved { get; }
//    bool IsRetrieving { get; } // Change type to IObservable<bool>
//    bool CanRetrieve { get; }
//    ITask<IEnumerable<TItem>> Retrieve(bool syncToCollection = true, CancellationToken cancellationToken = default);
//}
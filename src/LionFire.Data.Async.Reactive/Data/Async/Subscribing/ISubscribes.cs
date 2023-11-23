using DynamicData;

namespace LionFire.Subscribing;

public interface ISubscribes<T>
{
    IObservable<ChangeSet<T>> Changes { get; }

    ValueTask Subscribe();
    ValueTask Unsubscribe();
    bool IsSubscribed { get; set; }
}

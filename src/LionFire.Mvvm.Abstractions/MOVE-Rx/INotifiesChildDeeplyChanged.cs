
namespace LionFire.Mvvm;

public interface INotifiesChildDeeplyChanged<TItem>
{
    IObservable<(TItem, string[] propertyPath, object? oldValue)> ChildDeeplyChanged { get; }

}


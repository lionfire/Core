
namespace LionFire.Mvvm;

public interface INotifiesChildChanged<TItem>
{
    IObservable<(TItem, string? propertyName, object? oldValue)> ChildChanged { get; }
}


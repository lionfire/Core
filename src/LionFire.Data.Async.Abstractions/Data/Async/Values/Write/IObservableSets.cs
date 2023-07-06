
namespace LionFire.Data.Sets;

public interface IObservableSets<out TValue>
{
    IObservable<ISetOperation<TValue>> Sets { get; }
}



namespace LionFire.Data.Async.Sets;

public interface IObservableSetState
{
    IObservable<WriteState> WriteStates { get; }
}

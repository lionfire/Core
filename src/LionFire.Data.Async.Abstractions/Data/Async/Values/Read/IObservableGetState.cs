namespace LionFire.Data.Async.Gets;

public interface IObservableGetState
{
    IObservable<ReadState> ReadStates { get; }
}

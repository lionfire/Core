namespace LionFire.Data.Gets;

public interface IObservableGets<out TValue>
{
    IObservable<ILazyGetResult<TValue>> GetOperations { get; }
}

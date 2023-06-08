namespace LionFire.Mvvm;

public interface IResolvesCollectionVM<TValue, TValueVM, TCollection>
    : IResolvesRx<TCollection>
    where TCollection : IEnumerable<TValue>
{
}

namespace LionFire.Mvvm;

public interface IResolvesCollectionVM<TValue, TValueVM, TCollection>
    : IResolvesVM<TCollection>
    where TCollection : IEnumerable<TValue>
{
}

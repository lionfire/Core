

namespace LionFire.Data.Async.Mvvm;

public interface IResolvesCollectionVM<TValue, TValueVM, TCollection>
    : IGetsVM<TCollection>
    where TCollection : IEnumerable<TValue>
{
}

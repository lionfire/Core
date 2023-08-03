

using LionFire.Data.Async.Gets.Mvvm;

namespace LionFire.Data.Mvvm;

public interface IGetsCollectionVM<TValue, TValueVM, TCollection> // RENAME ICollectionGetterVM?
    : IStatelessGetterVM<TCollection>
    where TCollection : IEnumerable<TValue>
{
}

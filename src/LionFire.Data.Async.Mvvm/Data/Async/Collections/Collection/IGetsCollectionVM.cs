

using LionFire.Data.Gets.Mvvm;

namespace LionFire.Data.Mvvm;

public interface IGetsCollectionVM<TValue, TValueVM, TCollection> // RENAME ICollectionGetterVM?
    : IStatelessGetsVM<TCollection>
    where TCollection : IEnumerable<TValue>
{
}

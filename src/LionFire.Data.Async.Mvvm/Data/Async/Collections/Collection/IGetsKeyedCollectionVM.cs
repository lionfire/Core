namespace LionFire.Data.Mvvm;

public interface IGetsKeyedCollectionVM<TKey, TValue, TValueVM, TCollection>
    : IGetsCollectionVM<TValue, TValueVM, TCollection>
    where TCollection : IEnumerable<TValue>
{
    Func<TValue, TKey> KeySelector { get; }
}

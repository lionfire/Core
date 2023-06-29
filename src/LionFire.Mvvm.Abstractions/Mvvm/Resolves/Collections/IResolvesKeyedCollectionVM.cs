namespace LionFire.Data.Mvvm;

public interface IResolvesKeyedCollectionVM<TKey, TValue, TValueVM, TCollection>
    : IResolvesCollectionVM<TValue, TValueVM, TCollection>
    where TCollection : IEnumerable<TValue>
{
    Func<TValue, TKey> KeySelector { get; }
}

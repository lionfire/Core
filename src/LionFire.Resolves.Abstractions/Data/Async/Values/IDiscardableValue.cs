namespace LionFire.Data.Async;

public interface IDiscardableValue
{
    /// <summary>
    /// Discards the value that was either
    ///  - (for read objects) was resolved from a store (if applicable, possibly resulting in another resolve the next time a lazily resolved value is requested), 
    ///  - (for write objects) was staged to be written to a store -- also discarding any pending delete operation.
    /// </summary>
    void DiscardValue();
}

//public interface INotifyingLazilyResolves // Use Persistence instead?
//{
//    public event Action<ILazilyGets> Resolved;
//    public event Action<ILazilyGets> Discarded;
//}

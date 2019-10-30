namespace LionFire.Resolves
{
    public interface IDiscardableValue
    {
        /// <summary>
        /// Discards the value that was resolved from a store (if applicable, possibly resulting in another resolve the next time a lazily resolved value is requested), 
        /// and discards the value set by the user (if applicable) including any pending delete.
        /// </summary>
        void DiscardValue();
    }

    //public interface INotifyingLazilyResolves // Use Persistence instead?
    //{
    //    public event Action<ILazilyResolves> Resolved;
    //    public event Action<ILazilyResolves> Discarded;
    //}
}

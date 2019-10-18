namespace LionFire.Resolvables
{    
    public interface IResolvesEx : IResolves
    {
        /// <summary>
        /// Determine whether this object has a Value without triggering any lazy loading
        /// </summary>
        bool HasValue { get; }
    }
}

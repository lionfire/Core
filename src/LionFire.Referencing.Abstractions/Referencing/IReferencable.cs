namespace LionFire.Referencing
{
    /// <summary>
    /// If the reference can change, expose IChangeableReference.
    /// </summary>
    public interface IReferencable
    {
        IReference Reference { get; }
    }
    
}

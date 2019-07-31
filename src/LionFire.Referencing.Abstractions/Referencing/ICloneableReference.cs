namespace LionFire.Referencing
{
    /// <summary>
    /// Implement this for more efficient path traversal by cloning instances of IReference with a new path.
    /// </summary>
    public interface ICloneableReference
    {
        IReference Clone();
        IReference CloneWithPath(string newPath);
    }

}

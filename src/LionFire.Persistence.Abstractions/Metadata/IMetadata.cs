namespace LionFire.Persistence
{
    /// <summary>
    /// Wrapper to indicate a type is metadata, and not directly stored by a Persister.  The Persister should persist it via native metadata means, or set aside some privately reserved namespace 
    /// that it can use to store metadata.
    /// </summary>
    public interface IMetadata { }
}

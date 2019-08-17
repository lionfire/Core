namespace LionFire.Persistence
{
    /// <summary>
    /// Analogous to a file entry in a filesystem's directory listing.  Contains a primary key (such as a filename)
    /// plus a potential for some metadata that is stored alongside the key in the listing of children of the directory.
    /// </summary>
    public interface ICollectionEntry
    {
        /// <summary>
        /// Primary key for the child.  Used as a path segment in References to identify children of a collection.
        /// </summary>
        string Name { get; }
    }
}

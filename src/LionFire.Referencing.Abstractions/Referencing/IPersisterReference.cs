namespace LionFire.Referencing
{
    public interface IPersisterReference
    {
        /// <summary>
        /// Name of the persister to be used for this reference.  For example, each SQL database might have its own name, which in turn is configured with a paricular connection string.  Some handle providers, such as Filesystem, may have a default persister with a null (or empty) name.
        /// </summary>
        string Persister { get; }
    }
}

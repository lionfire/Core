namespace LionFire.Referencing
{
    public interface IOBaseReference : IReferenceWithLocation
    {
#if LEGACY // TOMIGRATE
        /// <summary>
        /// Specialized implementations of IReference may be tied to a particular ObjectStoreProvider: Eg. FileReference may be tied to FilesystemObjectStoreProvider
        /// </summary>
        IOBaseProvider DefaultObjectStoreProvider { get; }
#endif
    }
}

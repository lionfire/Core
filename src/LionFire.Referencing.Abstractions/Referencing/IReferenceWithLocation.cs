namespace LionFire.Referencing
{
    public interface IReferenceWithLocation : IReference
    {
        /// <summary>
        /// Like a connection string, or reference to a connection string.  Specifies OBaseProvider-specific detail about location. E.g. for VobReference, specifies a particular store name of a mount.
        /// For filesystem, this is ignored.  For DbReference, this specifies the database name.
        /// </summary>
        string Location { get; }
    }
}

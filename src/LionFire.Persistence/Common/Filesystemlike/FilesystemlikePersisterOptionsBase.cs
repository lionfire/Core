using LionFire.Serialization;

namespace LionFire.Persistence.Filesystemlike
{

    /// <summary>
    /// For filesystem-like Persistence
    /// </summary>
    public class FilesystemlikePersisterOptionsBase : PersistenceOptions
    {
        public string RootDirectory { get; set; }

        public string EndOfNameMarker { get; set; } = "'"; // REVIEW

        public AutoAppendExtension AutoAppendExtensionOnWrite { get; set; } = AutoAppendExtension.Disabled;
        public AppendExtensionOnRead AppendExtensionOnRead { get; set; } = AppendExtensionOnRead.Never;

        public ValidateOneFilePerPath ValidateOneFilePerPath { get; set; } = ValidateOneFilePerPath.None;

        /// <summary>
        /// If true, attempt to deserialize enough of the object to determine that the correct type is being deleted.
        /// It is safer to have this true than false, but it is safer to never save unexpected types at the same filename.
        /// </summary>
        public bool VerifyExistsAsTypeBeforeDelete { get; set; } = true;

        /// <summary>
        /// Lets you know whether the object existed with a Found or NotFound result flag.
        /// RENAME - DetectAnythingDeleted
        /// MOVE - to child classes that need it
        /// </summary>
        public bool VerifyExistsBeforeDelete { get; set; } = true;

        public bool AutoCreateParentDirectories { get; set; } = true;

    }
}

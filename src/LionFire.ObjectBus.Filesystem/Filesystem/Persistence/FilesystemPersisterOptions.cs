using LionFire.Dependencies;
using LionFire.Serialization;
using LionFire.Structures;
using System.Collections.Generic;

namespace LionFire.Persistence.Filesystem
{
    public class FilesystemPersisterOptions : FilesystemPersisterOptionsBase
    {
        //public static FSPersistenceOptions Current { get; } = DependencyLocator.Get<FSPersistenceOptions>();

        public string RootDirectory { get; set; }

        public FilesystemPersisterOptions()
        {
            MaxGetRetries = 10;
        }

        public static readonly bool AutoDeleteEmptyFiles = true;

        // FUTURE? Delete if file is all null (saw this on my SSDs after a machine crash) - TODO: some sort of null detection feature, and not FS-specific
        //public static readonly bool AutoDeleteNullFiles = true;

        public List<IFilesystemPersistenceInterceptor> Interceptors => interceptors;
        private readonly List<IFilesystemPersistenceInterceptor> interceptors = new List<IFilesystemPersistenceInterceptor>();

        public bool AutoCreateParentDirectories { get; set; } = true;

        /// <summary>
        /// If true, attempt to deserialize enough of the object to determine that the correct type is being deleted.
        /// It is safer to have this true than false, but it is safer to never save unexpected types at the same filename.
        /// </summary>
        public bool VerifyExistsAsTypeBeforeDelete { get; set; } = true;

        /// <summary>
        /// Probably pointless for Filesystems, unless you want to know whether the file existed and get a Found or NotFound
        /// </summary>
        public bool VerifyExistsBeforeDelete { get; set; } = true;
    }
}

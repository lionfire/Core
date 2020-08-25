using LionFire.Dependencies;
using LionFire.Persistence.Filesystemlike;
using LionFire.Serialization;
using LionFire.Structures;
using System.Collections.Generic;

namespace LionFire.Persistence.Filesystem
{
    public class FilesystemPersisterOptions : FilesystemlikePersisterOptionsBase
    {

        public FilesystemPersisterOptions()
        {
            RetryOptions.MaxGetRetries = 10;
        }

        public static readonly bool AutoDeleteEmptyFiles = true;

        public PersisterRetryOptions RetryOptions { get; set; } = new PersisterRetryOptions();

        // FUTURE? Delete if file is all null (saw this on my SSDs after a machine crash) - TODO: some sort of null detection feature, and not FS-specific
        //public static readonly bool AutoDeleteNullFiles = true;

        public List<IFilesystemPersistenceInterceptor> Interceptors => interceptors;
        private readonly List<IFilesystemPersistenceInterceptor> interceptors = new List<IFilesystemPersistenceInterceptor>();
        
    }
}

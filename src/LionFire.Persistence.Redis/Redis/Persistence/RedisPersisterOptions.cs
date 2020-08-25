using LionFire.Dependencies;
using LionFire.Persistence.Filesystemlike;
using LionFire.Serialization;
using LionFire.Structures;
using System.Collections.Generic;

namespace LionFire.Persistence.Redis
{
    public class RedisPersisterOptions : FilesystemlikePersisterOptionsBase
    {
        public string RootDirectory { get; set; }

        public RedisPersisterOptions()
        {
        }

        //public bool AutoCreateParentDirectories { get; set; } = true;

        /// <summary>
        /// Probably pointless for Redis, unless you want to know whether the file existed and get a Found or NotFound
        /// </summary>
        public bool VerifyExistsBeforeDelete { get; set; } = true;
    }
}

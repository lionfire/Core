#if TOPORT
using LionFire.ObjectBus.Filesystem;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Filesystem
{
    public class FileReadWriteHandle<TValue> : PersisterReadWriteHandle<FileReference, TValue, FilesystemPersister>
    {
        public FileReadWriteHandle(FileReference reference) : base(reference)
        {
        }
    }
}
#endif

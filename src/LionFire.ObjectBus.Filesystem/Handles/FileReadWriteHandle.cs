using LionFire.ObjectBus.Filesystem;
using LionFire.Persistence.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Filesystem
{
    public class FileReadWriteHandle<TValue> : PersisterSingletonReadWriteHandle<FileReference, TValue, FilesystemPersister>
    {
        public FileReadWriteHandle(FileReference reference) : base(reference)
        {
        }
    }
}

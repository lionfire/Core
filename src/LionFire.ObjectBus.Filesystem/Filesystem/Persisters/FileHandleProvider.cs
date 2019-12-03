using LionFire.Persistence.Handles;

namespace LionFire.Persistence.Filesystem
{
    public class FileHandleProvider : IReadHandleProvider<FileReference>, IReadWriteHandleProvider<FileReference>
    {
        public IReadHandleBase<T> GetReadHandle<T>(FileReference reference) => new PersisterSingletonReadWriteHandle<FileReference, T, FilesystemPersister>(reference);
        public IReadWriteHandleBase<T> GetReadWriteHandle<T>(FileReference reference) => new PersisterSingletonReadWriteHandle<FileReference, T, FilesystemPersister>(reference);
    }
}

using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemBinaryHandleProvider : FilesystemHandleProviderBase, IReadWriteHandleProvider, IReadHandleProvider
    {
        public override string Scheme => "file(byte[])";

        public IReadWriteHandle<T> GetReadWriteHandle<T>(IReference reference)
        {
            ValidateReference(reference);
            return (IReadWriteHandle<T>)(object)new HBinaryFile(reference.Path);
        }

        public IReadHandle<T> GetReadHandle<T>(IReference reference)
        {
            ValidateReference(reference);
            return (IReadHandle<T>)(object)new RBinaryFile(reference.Path);
        }

        public IReadWriteHandle<T> GetReadWriteHandle<T>(IReference reference, T preresolvedValue = default) => throw new System.NotImplementedException();
        public IReadHandle<T> GetReadHandle<T>(IReference reference, T preresolvedValue = default) => throw new System.NotImplementedException();
    }
}

using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemBinaryHandleProvider : FilesystemHandleProviderBase, IReadWriteHandleProvider, IReadHandleProvider
    {
        public override string Scheme => "file(byte[])";

        public IReadWriteHandleBase<T> GetReadWriteHandle<T>(IReference reference, T obj = default)
        {
            ValidateReference(reference);
            return (IReadWriteHandleBase<T>)(object)new HBinaryFile(reference.Path);
        }

        public IReadHandleBase<T> GetReadHandle<T>(IReference reference, T obj = default)
        {
            ValidateReference(reference);
            return (IReadHandleBase<T>)(object)new RBinaryFile(reference.Path);
        }
    }


}

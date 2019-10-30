using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemBinaryHandleProvider : FilesystemHandleProviderBase, IReadWriteHandleProvider, IReadHandleProvider
    {
        public override string Scheme => "file(byte[])";

        public W<T> GetReadWriteHandle<T>(IReference reference, T obj = default)
        {
            ValidateReference(reference);
            return (W<T>)(object)new HBinaryFile(reference.Path);
        }

        public RH<T> GetReadHandle<T>(IReference reference, T obj = default)
        {
            ValidateReference(reference);
            return (RH<T>)(object)new RBinaryFile(reference.Path);
        }
    }


}

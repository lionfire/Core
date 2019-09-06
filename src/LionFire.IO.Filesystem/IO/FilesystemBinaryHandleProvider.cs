using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemBinaryHandleProvider : FilesystemHandleProviderBase, IHandleProvider, IReadHandleProvider
    {
        public override string Scheme => "file(byte[])";

        public H<T> GetHandle<T>(IReference reference, T obj = default)
        {
            ValidateReference(reference);
            return (H<T>)(object)new HBinaryFile(reference.Path);
        }

        public RH<T> GetReadHandle<T>(IReference reference, T obj = default)
        {
            ValidateReference(reference);
            return (RH<T>)(object)new RBinaryFile(reference.Path);
        }
    }


}

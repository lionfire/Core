using LionFire.Referencing;

namespace LionFire.IO.Filesystem
{
    public class FilesystemBinaryHandleProvider : FilesystemHandleProviderBase, IHandleProvider
    {
        public override string Scheme => "file(byte[])";

        public H<T> GetHandle<T>(IReference reference)
        {
            ValidateReference(reference);
            return (H<T>)(object)new HBinaryFile(reference.Path);
        }

        public RH<T> GetReadHandle<T>(IReference reference)
        {
            ValidateReference(reference);
            return (RH<T>)(object)new RBinaryFile(reference.Path);
        }
    }


}

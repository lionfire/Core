using System;
using System.IO;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemStreamWHandleProvider : FilesystemHandleProviderBase, IHandleProvider
    {
        public override string Scheme => "file(WStream)";

        public H<T> GetHandle<T>(IReference reference)
        {
            if (typeof(T) != typeof(Stream))
            {
                throw new ArgumentException("Object type must be Stream");
            }

            ValidateReference(reference);
            return (H<T>)new WFileStream(reference.Path);
        }

        public RH<T> GetReadHandle<T>(IReference reference) => throw new NotSupportedException();
    }


}

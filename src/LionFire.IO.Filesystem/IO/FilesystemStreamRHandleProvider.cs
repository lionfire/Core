using System;
using System.IO;
using LionFire.Referencing;

namespace LionFire.IO.Filesystem
{
    public class FilesystemStreamReadHandleProvider : FilesystemHandleProviderBase, IReadHandleProvider
    {
        public override string Scheme => "file-stream-ro";

        public RH<T> GetReadHandle<T>(IReference reference)
        {
            if (typeof(T) != typeof(Stream))
            {
                throw new ArgumentException("Object type must be Stream");
            }

            ValidateReference(reference);
            return (RH<T>)new RFileStream(reference.Path);
        }
    }


}

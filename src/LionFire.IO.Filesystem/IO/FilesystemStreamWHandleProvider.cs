using System;
using System.IO;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemStreamWHandleProvider : FilesystemHandleProviderBase, IReadWriteHandleProvider
    {
        public override string Scheme => "file(WStream)";

        public IReadWriteHandle<T> GetReadWriteHandle<T>(IReference reference)
        {
            if (typeof(T) != typeof(Stream))
            {
                throw new ArgumentException("Object type must be Stream");
            }

            ValidateReference(reference);
            return (IReadWriteHandle<T>)new WFileStream(reference.Path); // HARDCAST
        }

        public IReadHandle<T> GetReadHandle<T>(IReference reference) => throw new NotSupportedException();
    }


}

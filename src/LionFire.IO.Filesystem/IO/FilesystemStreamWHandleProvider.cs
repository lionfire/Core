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

        public W<T> GetReadWriteHandle<T>(IReference reference, T initialObject = default)
        {
            if (typeof(T) != typeof(Stream))
            {
                throw new ArgumentException("Object type must be Stream");
            }

            ValidateReference(reference);
            return (W<T>)new WFileStream(reference.Path, (Stream)(object)initialObject); // HARDCAST
        }

        public RH<T> GetReadHandle<T>(IReference reference, T initialObject = default) => throw new NotSupportedException();
    }


}

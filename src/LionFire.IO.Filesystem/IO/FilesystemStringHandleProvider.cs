using System;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemStringHandleProvider : FilesystemHandleProviderBase, IReadWriteHandleProvider
    {
        public override string Scheme => "file(string)";

        public IReadWriteHandle<T> GetReadWriteHandle<T>(IReference reference)
        {
            if (typeof(T) != typeof(string))
            {
                throw new ArgumentException("Object type must be string");
            }

            ValidateReference(reference);
            return (IReadWriteHandle<T>)new HTextFile(reference.Path); // HARDCAST
        }

        public IReadHandle<T> GetReadHandle<T>(IReference reference, T preresolvedValue = default)
        {
            if (typeof(T) != typeof(string))
            {
                throw new ArgumentException("Object type must be string");
            }

            ValidateReference(reference);
            return (IReadHandle<T>)new RTextFile(reference.Path);
        }
    }
}

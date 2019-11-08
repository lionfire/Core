using System;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemStringHandleProvider : FilesystemHandleProviderBase, IReadWriteHandleProvider
    {
        public override string Scheme => "file(string)";

        public IReadWriteHandleBase<T> GetReadWriteHandle<T>(IReference reference, T initialObject = default)
        {
            if (typeof(T) != typeof(string))
            {
                throw new ArgumentException("Object type must be string");
            }

            ValidateReference(reference);
            return (IReadWriteHandleBase<T>)new HTextFile(reference.Path, (string)(object)initialObject); // HARDCAST
        }

        public IReadHandleBase<T> GetReadHandle<T>(IReference reference)
        {
            if (typeof(T) != typeof(string))
            {
                throw new ArgumentException("Object type must be string");
            }

            ValidateReference(reference);
            return (IReadHandleBase<T>)new RTextFile(reference.Path);
        }
    }


}

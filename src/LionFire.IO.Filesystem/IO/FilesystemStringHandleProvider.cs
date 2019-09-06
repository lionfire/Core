using System;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using LionFire.Persistence;

namespace LionFire.IO.Filesystem
{
    public class FilesystemStringHandleProvider : FilesystemHandleProviderBase, IHandleProvider
    {
        public override string Scheme => "file(string)";

        public H<T> GetHandle<T>(IReference reference, T initialObject = default)
        {
            if (typeof(T) != typeof(string))
            {
                throw new ArgumentException("Object type must be string");
            }

            ValidateReference(reference);
            return (H<T>)new HTextFile(reference.Path, (string)(object)initialObject); // HARDCAST
        }

        public RH<T> GetReadHandle<T>(IReference reference)
        {
            if (typeof(T) != typeof(string))
            {
                throw new ArgumentException("Object type must be string");
            }

            ValidateReference(reference);
            return (RH<T>)new RTextFile(reference.Path);
        }
    }


}

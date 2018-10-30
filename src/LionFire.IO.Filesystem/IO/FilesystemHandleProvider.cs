using System;
using System.IO;
using LionFire.Referencing;

namespace LionFire.IO.Filesystem
{
    public abstract class FilesystemHandleProviderBase
    {
        public abstract string Scheme { get; } // e.g. "file(something)";

        protected void ValidateReference(IReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException($"{nameof(reference)}");
            }

            if (reference.Scheme != Scheme)
            {
                throw new ArgumentException($"Reference scheme must be {Scheme}");
            }
        }
    }
    public class FilesystemBinaryHandleProvider : FilesystemHandleProviderBase, IHandleProvider
    {
        public override string Scheme => "file(byte[])";

        public H<T> GetHandle<T>(IReference reference)
            where T : class
        {
            ValidateReference(reference);
            return (H<T>)(object)new HBinaryFile(reference.Path);
        }
    }
    public class FilesystemStringHandleProvider : FilesystemHandleProviderBase, IHandleProvider
    {
        public override string Scheme => "file(string)";

        public H<T> GetHandle<T>(IReference reference)
            where T : class
        {
            if (typeof(T) != typeof(string)) throw new ArgumentException("Object type must be string");
            ValidateReference(reference);
            return (H<T>)(object)new HTextFile(reference.Path);
        }
    }
    public class FilesystemStreamRHandleProvider : FilesystemHandleProviderBase, IHandleProvider
    {
        public override string Scheme => "file(RStream)";

        public H<T> GetHandle<T>(IReference reference)
            where T : class
        {
            if (typeof(T) != typeof(Stream)) throw new ArgumentException("Object type must be Stream");
            ValidateReference(reference);
            return (H<T>)(object)new RFileStream(reference.Path);
        }
    }

    public class FilesystemStreamWHandleProvider : FilesystemHandleProviderBase, IHandleProvider
    {
        public override string Scheme => "file(WStream)";

        public H<T> GetHandle<T>(IReference reference)
            where T : class
        {
            if (typeof(T) != typeof(Stream)) throw new ArgumentException("Object type must be Stream");
            ValidateReference(reference);
            return (H<T>)(object)new WFileStream(reference.Path);
        }
    }


}

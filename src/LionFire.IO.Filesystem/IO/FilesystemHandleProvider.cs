using System;
using LionFire.Referencing;

namespace LionFire.IO.Filesystem
{
    public abstract class FilesystemHandleProviderBase
    {
        public abstract string Scheme { get; } // e.g. "file-something";

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
}

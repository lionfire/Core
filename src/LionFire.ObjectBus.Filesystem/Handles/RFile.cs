using System;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Filesystem
{
    public class RFile<T> : RBase<T>
        where T : class
    {
        public RFile() { }
        public RFile(string path) : base(new LocalFileReference(path))
        {
        }
        public RFile(LocalFileReference fileReference) : base(fileReference)
        {
        }

        public override Task<bool> TryRetrieveObject() => throw new NotImplementedException();
    }
}

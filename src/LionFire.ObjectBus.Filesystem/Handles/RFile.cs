using System;
using System.Threading.Tasks;
using LionFire.Persistence.Handles;
using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Filesystem
{
    public class RFile<T> : ReadHandle<T>
        where T : class
    {
        public RFile() { }
        public RFile(string path) : base(new FileReference(path))
        {
        }
        public RFile(FileReference fileReference) : base(fileReference)
        {
        }

        public override Task<IRetrieveResult<T>> RetrieveImpl() => throw new NotImplementedException();
    }
}

using LionFire.Referencing;

namespace LionFire.Persistence.Filesystem
{
    public class BaseFileReference : BaseReference<FileReference>
    {
        public BaseFileReference(FileReference reference)
        {
            this.reference = reference;
        }

        public override FileReference Reference => reference;
        private readonly FileReference reference;

        public override int Count => 1;

        public override IReferencable<FileReference> AddRight(IReference right) 
            => new FileReference(LionPath.Combine(reference.Path, right.Path));
    }

}

using LionFire.Referencing;
using System.Collections.Generic;

namespace LionFire.Persistence.Filesystem
{
    public class FileReferenceProvider : ReferenceProviderBase<FileReference>
    {
        public override string UriScheme => "file";

        public override (FileReference reference, string error) TryGetReference(string path)
            => (new FileReference(path), null);
    }

}

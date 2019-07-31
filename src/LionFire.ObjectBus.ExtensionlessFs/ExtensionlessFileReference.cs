using System;
using System.Collections.Generic;
using LionFire.ObjectBus.Filesystem;
using LionFire.Referencing;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public class ExtensionlessFileReference : LocalReferenceBase
    {
        
        public const string UriScheme = "efile";
        public const string UriPrefixDefault = "efile:///";
        public override IEnumerable<string> AllowedSchemes { get { yield return UriScheme; } }

        public override string Scheme => UriScheme;
       

        public override string Path
        {
            get => path;
            set
            {
                if (path != null) throw new AlreadySetException();
                path = LocalFileReference.CoercePath(value);
            }
        }
        private string path;
        
        // Implementation similar to LocalFileReference
        public override string ToString() => String.Concat(UriPrefixDefault, Path);
        internal static IReference TryCreate(string referenceString) => throw new NotImplementedException();

        // Implementation similar to LocalFileReference
        public override string Key => this.ToString();

    }
}

using LionFire.Persistence.Filesystem;

namespace LionFire.Persistence.AutoExtensionFilesystem
{
    public class AutoExtensionFileReference : FileReference<AutoExtensionFileReference> // REVIEW: which base class?
    {
        public AutoExtensionFileReference() { }
        public AutoExtensionFileReference(string path) : base(path) { }
        public static implicit operator AutoExtensionFileReference(string path) => new AutoExtensionFileReference(path);

        public new static class Constants
        {
            public const string UriScheme = "efile";
        }
        public override string UriPrefixDefault => "efile:///";

        public override string UriSchemeColon => "efile:";

        public string UriScheme =>  Constants.UriScheme;

        public override string Scheme => UriScheme;
    }
}

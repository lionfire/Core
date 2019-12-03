using System;
using LionFire.Serialization;

namespace LionFire.Serialization
{
    public enum FileExtensionScoring
    {
        Unspecified,

        /// <summary>
        /// Recommended if you can use the proper file extensions
        /// </summary>
        MustMatch,

        RewardMatch,
        IgnoreExtension,
    }

    public class SerializationOptions
    {
        public SerializationFlags SerializationFlags { get; set; } = SerializationFlags.HumanReadable | SerializationFlags.Text;


        // TODO: Reconcile this with the two DependencyLocator strategies?  Singletons vs IServiceProvider
        //public bool RequireSupportedExtensionsOnSerialize { get; set; } = true; // TODO: If true, use NaN/0 as fail/pass scores, instead of -10/+10
        //public bool RequireSupportedExtensionsOnDeserialize { get; set; } = true; // TODO: If true, use NaN/0 as fail/pass scores, instead of -10/+10

        public FileExtensionScoring SerializeExtensionScoring { get; set; } = FileExtensionScoring.MustMatch;

        // Not sure of the best way to map extensions to de/serializers and/or mime types and/or other file extensions.  Text deserializer could be a low-priority deserializer for XML files, for example, and perhaps medium for Markdown files.
        //public void RegisterTextExtensions(params string[] extensions) => throw new NotImplementedException();

        public FileExtensionScoring DeserializeExtensionScoring { get; set; } = FileExtensionScoring.MustMatch;

        public bool RequireMimeTypes { get; set; }
    }
}

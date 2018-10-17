using System.Collections.Generic;

namespace LionFire.Serialization
{
    public class SerializationFormat
    {
        /// <summary>
        /// E.g. JSON, Xml
        /// </summary>
        public string FormatName { get; set; }
        public IEnumerable<string> MimeTypes { get; }

        /// <summary>
        /// E.g. Readable, uglified
        /// </summary>
        public string FormatVariant { get; set; }

        public string Description { get; set; }

        public string FileExtensions { get; set; }
        public string DefaultFileExtension { get; set; }

    }
}

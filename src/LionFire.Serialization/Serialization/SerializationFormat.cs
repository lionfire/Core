using System.Collections.Generic;
using System.Linq;

namespace LionFire.Serialization
{
    public class SerializationFormat
    {
        #region Construction

        public SerializationFormat() { }
        public SerializationFormat(string fileExtension, string name = null, params string[] mimeTypes)
        {
            Name = name;
            //VariantName = variant;
            MimeTypes = mimeTypes;
            FileExtensions = new string[] { fileExtension };
        }

        #endregion

        /// <summary>
        /// E.g. JSON, Xml
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// E.g. Readable, uglified
        /// </summary>
        public string VariantName { get; set; }

        public IEnumerable<string> MimeTypes { get; }

        public string Description { get; set; }

        #region FileExtensions

        public IEnumerable<string> FileExtensions
        {
            get => fileExtensions ?? new string[] { DefaultFileExtension };
            set => fileExtensions = value;
        }
        private IEnumerable<string> fileExtensions;

        public string DefaultFileExtension => FileExtensions?.FirstOrDefault();

        #endregion

    }
}

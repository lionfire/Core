using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Filesystem
{

    [LionSerializable(SerializeMethod.ByValue)]
    public class FileReference : FileReferenceBase<FileReference>
    {
        #region Scheme

        public static class Constants
        {
            public const string UriScheme = "file";
        }
        public override string UriScheme => Constants.UriScheme;
        public override string UriPrefixDefault => "file:///";
        public override string UriSchemeColon => "file:";

        public static readonly IEnumerable<string> UriSchemes = new string[] { Constants.UriScheme };

        public override string Scheme => UriScheme;

        #endregion

        #region Construction and Implicit Construction

        public FileReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public FileReference(string path) : base(path)
        {
        }

        //public LocalFileReference(IReference reference)
        //{
        //    ValidateCanConvertFrom(reference);
        //    CopyFrom(reference);
        //}

        public static implicit operator FileReference(string path) => ToReference(path);

        public static FileReference ToReference(string path) => new FileReference(path);

        public static FileReference ReferenceFromKey(string path) => ToReference(path);

        public static string ToReferenceKey(string path) => path;

        #endregion

        #region Conversion

        public static void ValidateCanConvertFrom(IReference reference)
        {

            if (reference.Scheme != Constants.UriScheme)
            {
                throw new ReferenceException("UriScheme not supported");
            }
        }

        public static FileReference ConvertFrom(IReference parent)
        {
            var fileRef = parent as FileReference;

            if (fileRef == null && parent.Scheme == Constants.UriScheme)
            {
                fileRef = new FileReference(parent.Path);
            }

            return fileRef;
        }

        #endregion

    }

    public static class FileReferenceExtensions
    {
        public static FileReference ToFileReference(this string path) => new FileReference(path);
    }

}

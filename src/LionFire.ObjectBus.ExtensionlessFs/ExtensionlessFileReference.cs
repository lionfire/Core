using System;
using System.Collections.Generic;
using LionFire.ObjectBus.Filesystem;
using LionFire.Referencing;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public class ExtensionlessFileReference : FileReferenceBase<ExtensionlessFileReference>
    {
        public class Constants
        {
            public const string UriScheme = "filename";
        }
        public override string UriScheme => Constants.UriScheme;
        public override string UriPrefixDefault => "efile:///";
        public override string UriSchemeColon => "efile:";
        public static readonly IEnumerable<string> UriSchemes = new string[] { Constants.UriScheme };

        public override string Scheme => UriScheme;

        // Duplicate with LocalFileReference -----

        //public override string Key { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        //public override string Path
        //{
        //    get => path;
        //    set
        //    {
        //        if (path != null) throw new AlreadySetException();
        //        path = LocalFileReference.CoercePath(value);
        //    }
        //}

        // Implementation similar to LocalFileReference
        //public override string ToString() => String.Concat(UriPrefixDefault, Path);
        //internal static IReference TryCreate(string referenceString) => throw new NotImplementedException();

        // Implementation similar to LocalFileReference
        //public override string Key => this.ToString();

        #region Construction and Implicit Construction

        public ExtensionlessFileReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public ExtensionlessFileReference(string path) : base(path)
        {
        }

        //public LocalFileReference(IReference reference)
        //{
        //    ValidateCanConvertFrom(reference);
        //    CopyFrom(reference);
        //}

        public static implicit operator ExtensionlessFileReference(string path) => ToReference(path);

        public static ExtensionlessFileReference ToReference(string path) => new ExtensionlessFileReference(path);

        public static ExtensionlessFileReference ReferenceFromKey(string path) => ToReference(path);

        public static string ToReferenceKey(string path) => path;

        #endregion

        #region Conversion

        public static void ValidateCanConvertFrom(IReference reference)
        {
            if (reference.Scheme != Constants.UriScheme)
            {
                throw new OBusReferenceException("UriScheme not supported");
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
}

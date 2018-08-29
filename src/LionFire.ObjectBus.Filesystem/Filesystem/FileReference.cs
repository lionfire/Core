using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.ObjectBus.Filesystem
{

    [LionSerializable(SerializeMethod.ByValue)]
    public class FileReference : ReferenceBase<FileReference>
    {

        #region Construction and Implicit Construction

        public FileReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public FileReference(string path)
        {
            this.Path = path;
        }

        public FileReference(IReference reference)
        {
            ValidateCanConvertFrom(reference);
            CopyFrom(reference);
        }

        public static implicit operator FileReference(string path)
        {
            return new FileReference(path);
        }

        #endregion

        #region Conversion and Implicit operators

        //public static implicit operator Handle(FileReference fsref)
        //{
        //    return fsref.ToHandle();
        //}

        #endregion

        #region Conversion

        public static void ValidateCanConvertFrom(IReference reference)
        {
            if (reference.Scheme == UriScheme)
            {
                throw new OBusReferenceException("UriScheme not valid");
            }
        }

        public static FileReference ConvertFrom(IReference parent)
        {
            FileReference fileRef = parent as FileReference;

            if (fileRef == null && parent.Scheme == UriScheme)
            {
                fileRef = new FileReference(parent.Path);
            }

            return fileRef;
        }

        #endregion

        #region Scheme

        public const string UriScheme = "file";
        public const string UriPrefixDefault = "file://";
        public static readonly string[] UriSchemes = new string[] { UriScheme };

        public override string Scheme
        {
            get
            {
                return UriScheme;
            }
            set { throw new NotSupportedException(); }
        }

        #endregion

        public override string Path
        {
            get { return base.Path; }
            set
            {
#if MONO
                value = value.Replace('\\', '/');
#else
                value = value.Replace('/', '\\');
#endif


                if (value != null)
                {
                    if (value.Length >= 1)
                    {
                        if (value[0] == ':') throw new ArgumentException();
                    }
                    
                    var colon = value.LastIndexOf(':');
                    if (colon != -1 && colon != 1)
                    {
                        throw new ArgumentException();
                    }
                }

                base.Path = value;
            }
        }

        public override IOBaseProvider DefaultObjectStoreProvider
        {
            get
            {
                return FsOBaseProvider.Instance;
            }
        }

        public override string ToString()
        {
            return String.Concat(UriPrefixDefault, Path);
        }

        public override string Key
        {
            get
            { return this.ToString(); }
            set
            { throw new NotImplementedException("set_Key"); }
        }
    }

    public static class FileReferenceExtensions
    {
        public static FileReference AsFileReference(this string path)
        {
            return new FileReference(path);
        }
    }
}

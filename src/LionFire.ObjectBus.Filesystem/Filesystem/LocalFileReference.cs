using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.ObjectBus.Filesystem
{

    //public static class testext
    //{
    //    public static T ZZ<T>(this T z)
    //    {
    //        return z;
    //    }
    //    public static ConcreteType GetChild<ConcreteType>(this ConcreteType reference, string subPath)
    //        where ConcreteType : IReference
    //    {
    //        return (ConcreteType)base.GetChild(subPath);
    //    }
    //}
    [LionSerializable(SerializeMethod.ByValue)]
    public class LocalFileReference : LocalReferenceBase
    {

        #region Construction and Implicit Construction

        public LocalFileReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public LocalFileReference(string path)
        {
            this.Path = path;
        }

        //public LocalFileReference(IReference reference)
        //{
        //    ValidateCanConvertFrom(reference);
        //    CopyFrom(reference);
        //}

        public static implicit operator LocalFileReference(string path)
        {
            return new LocalFileReference(path);
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
            if (reference.Scheme != UriScheme)
            {
                throw new OBusReferenceException("UriScheme not supported");
            }
        }

        public static LocalFileReference ConvertFrom(IReference parent)
        {
            LocalFileReference fileRef = parent as LocalFileReference;

            if (fileRef == null && parent.Scheme == UriScheme)
            {
                fileRef = new LocalFileReference(parent.Path);
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
        }

        #endregion

        public override string Path
        {
            get { return path; }
            protected set
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

                path = value;
            }
        }
        private string path;

        //public override IOBaseProvider DefaultObjectStoreProvider
        //{
        //    get
        //    {
        //        return FsOBaseProvider.Instance;
        //    }
        //}

        public override string ToString() => String.Concat(UriPrefixDefault, Path);
        
        public override string Key => this.ToString();
        
    }

    public static class LocalFileReferenceExtensions
    {
        public static LocalFileReference AsLocalFileReference(this string path)
        {
            return new LocalFileReference(path);
        }
    }
}

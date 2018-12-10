using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.MultiTyping;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;

namespace LionFire.ObjectBus.RedisPub
{
    [LionSerializable(SerializeMethod.ByValue)]
    public class RedisPubReference : LocalReferenceBase, IHas<IOBase>, IHas<IOBus>
    {
        IOBus IHas<IOBus>.Object => ManualSingleton<RedisPubOBus>.GuaranteedInstance;
        IOBase IHas<IOBase>.Object => RedisPubOBase;
        RedisPubOBase RedisPubOBase => RedisPubOBase.DefaultInstance; // TODO: different hosts

        #region Construction and Implicit Construction

        public RedisPubReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public RedisPubReference(string path)
        {
            this.Path = path;
        }

        //public RedisPubReference(IReference reference)
        //{
        //    ValidateCanConvertFrom(reference);
        //    CopyFrom(reference);
        //}

        public static implicit operator RedisPubReference(string path)
        {
            return new RedisPubReference(path);
        }

        #endregion

        #region Conversion and Implicit operators

        //public static implicit operator Handle(RedisPubReference redisRef)
        //{
        //    return redisRef.ToHandle();
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

        public static RedisPubReference ConvertFrom(IReference parent)
        {
            RedisPubReference fileRef = parent as RedisPubReference;

            if (fileRef == null && parent.Scheme == UriScheme)
            {
                fileRef = new RedisPubReference(parent.Path);
            }

            return fileRef;
        }

        #endregion

        #region Scheme

        public const string UriScheme = "redis-pub";
        public const string UriPrefixDefault = "redis-pub:";
        public static readonly IEnumerable<string> UriSchemes = new string[] { UriScheme };
        public override IEnumerable<string> AllowedSchemes => UriSchemes;

        public override string Scheme => UriScheme;

        #endregion

        public override string Path
        {
            get { return path; }
            set
            {
//#if MONO
                value = value.Replace('\\', '/');
//#else
//                value = value.Replace('/', '\\');
//#endif

                //if (value != null)
                //{
                //    if (value.Length >= 1)
                //    {
                //        if (value[0] == ':') throw new ArgumentException();
                //    }

                //    var colon = value.LastIndexOf(':');
                //    if (colon != -1 && colon != 1)
                //    {
                //        throw new ArgumentException();
                //    }
                //}

                path = value;
            }
        }
        private string path;

        //public override IOBaseProvider DefaultObjectStoreProvider
        //{
        //    get
        //    {
        //        return RedisPubOBaseProvider.Instance;
        //    }
        //}

        public override string ToString() => String.Concat(UriPrefixDefault, Path);
        
        public override string Key => this.ToString();

    }

    public static class RedisPubReferenceExtensions
    {
        public static RedisPubReference AsRedisPubReference(this string path)
        {
            return new RedisPubReference(path);
        }
    }
}

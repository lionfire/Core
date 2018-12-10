using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.MultiTyping;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;

namespace LionFire.ObjectBus.Redis
{
    [LionSerializable(SerializeMethod.ByValue)]
    public class RedisReference : LocalReferenceBase, IHas<IOBase>, IHas<IOBus>
    {
        IOBus IHas<IOBus>.Object => ManualSingleton<RedisOBus>.GuaranteedInstance;
        IOBase IHas<IOBase>.Object => RedisOBase;
        RedisOBase RedisOBase => RedisOBase.DefaultInstance; // TODO: different hosts

        #region Construction and Implicit Construction

        public RedisReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public RedisReference(string path)
        {
            this.Path = path;
        }

        //public RedisReference(IReference reference)
        //{
        //    ValidateCanConvertFrom(reference);
        //    CopyFrom(reference);
        //}

        public static implicit operator RedisReference(string path)
        {
            return new RedisReference(path);
        }

        #endregion

        #region Conversion and Implicit operators

        //public static implicit operator Handle(RedisReference redisRef)
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

        public static RedisReference ConvertFrom(IReference parent)
        {
            RedisReference fileRef = parent as RedisReference;

            if (fileRef == null && parent.Scheme == UriScheme)
            {
                fileRef = new RedisReference(parent.Path);
            }

            return fileRef;
        }

        #endregion

        #region Scheme

        public const string UriScheme = "redis";
        public const string UriPrefixDefault = "redis:";
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
        //        return RedisOBaseProvider.Instance;
        //    }
        //}

        public override string ToString() => String.Concat(UriPrefixDefault, Path);
        
        public override string Key => this.ToString();

    }

    public static class RedisReferenceExtensions
    {
        public static RedisReference AsRedisReference(this string path)
        {
            return new RedisReference(path);
        }
    }
}

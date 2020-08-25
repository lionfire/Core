using System;
using System.Collections.Generic;
using LionFire.Persistence.Filesystem;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Redis
{
//#error NEXT: create a reference class for Redis.  How can we avoid making it 

    // REVIEW - Refactor this and other classes with Fs LionFire.Persistence.Filesystem
    // TODO: move base classes to LionFire.Persistence or LionFire.Persistence.Common

    [LionSerializable(SerializeMethod.ByValue)]
    public class RedisReference : LocalReferenceBase<RedisReference>
    {
        #region Scheme

        public static class Constants
        {
            public const string UriScheme = "redis";
        }
        public override string UriScheme => Constants.UriScheme;
        public override string UriPrefixDefault => "redis:///";
        public override string UriSchemeColon => "redis:";

        public static readonly IEnumerable<string> UriSchemes = new string[] { Constants.UriScheme };

        public override string Scheme => UriScheme;

        #endregion

        #region Construction and Implicit Construction

        public RedisReference() { }

        /// <summary>
        /// (Does not support URIs (TODO))
        /// </summary>
        /// <param name="path"></param>
        public RedisReference(string path) : base(path)
        {
        }

        //public LocalRedisReference(IReference reference)
        //{
        //    ValidateCanConvertFrom(reference);
        //    CopyFrom(reference);
        //}

        public static implicit operator RedisReference(string path) => ToReference(path);

        public static RedisReference ToReference(string path) => new RedisReference(path);

        public static RedisReference ReferenceFromKey(string path) => ToReference(path);

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

        public static RedisReference ConvertFrom(IReference parent)
        {
            var fileRef = parent as RedisReference;

            if (fileRef == null && parent.Scheme == Constants.UriScheme)
            {
                fileRef = new RedisReference(parent.Path);
            }

            return fileRef;
        }

        #endregion

    }

    public static class RedisReferenceExtensions
    {
        public static RedisReference ToRedisReference(this string path) => new RedisReference(path);
    }

}

using System;
using System.Collections.Generic;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Redis
{
    // REVIEW - Refactor this and other classes with Fs LionFire.Persistence.Filesystem

    public interface IRedisReference : IReference { }

    public class RedisReference : RedisReference<object>
    {

    }

    [LionSerializable(SerializeMethod.ByValue)]
    public class RedisReference<T> : LocalReferenceBase<RedisReference<T>, T>, IRedisReference
    {
        #region Scheme

        public static class Constants
        {
            public const string UriScheme = "redis";
        }

        public string UriScheme => Constants.UriScheme;
        //public override string UriPrefixDefault => "redis:///";
        //public override string UriSchemeColon => "redis:";

        public static readonly IEnumerable<string> UriSchemes = new string[] { Constants.UriScheme };

        public override string Scheme => UriScheme;

        public override IEnumerable<string> AllowedSchemes => UriSchemes;

        #endregion

        public override string Key { get =>path; protected set => base.Path = value; }

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

        public static implicit operator RedisReference<T>(string path) => ToReference(path);

        public static RedisReference<T> ToReference(string path) => new RedisReference<T>(path);

        public static RedisReference<T> ReferenceFromKey(string path) => ToReference(path);

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

        public static RedisReference<T> ConvertFrom(IReference parent)
        {
            var fileRef = parent as RedisReference<T>;

            if (fileRef == null && parent.Scheme == Constants.UriScheme)
            {
                fileRef = new RedisReference<T>(parent.Path);
            }

            return fileRef;
        }

        #endregion

    }

    public static class RedisReferenceExtensions
    {
        public static RedisReference<T> ToRedisReference<T>(this string path) => new RedisReference<T>(path);
    }

}

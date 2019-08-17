using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Redis
{
    public class RedisOBus : OBusBase<RedisOBus, RedisOBase, RedisReference>, IDefaultOBaseProvider
    {
        public override IOBase DefaultOBase => RedisOBase.DefaultInstance;

        //public override IEnumerable<Type> ReferenceTypes
        //{
        //    get
        //    {
        //        yield return typeof(RedisReference);
        //    }
        //}

        //public override IEnumerable<Type> HandleTypes
        //{
        //    get
        //    {
        //        yield return typeof(OBaseHandle<>);
        //    }
        //}

        //public override IEnumerable<string> UriSchemes => RedisReference.UriSchemes;

        //public bool IsValid(IReference reference) => reference.GetOBases().Any();

        //public IOBase GetOBaseForConnectionString(string connectionString)
        //{
        //    if (connectionString != null)
        //    {
        //        throw new ArgumentException("connectionString must be null");
        //    }

        //    // For now, there is only one instance, representing the local file system.
        //    return RedisOBase.Instance;
        //}

        public override IReference TryGetReference(string uri)
        {
            int colonIndex = uri.IndexOf(':');
            if (colonIndex < 0)
            {
                return null;
                //throw new ArgumentException("Scheme missing");
            }

            #region // TODO: Verify scheme is supported!

            if (!uri.StartsWith(RedisReference.UriScheme))
            {
                return null;
                //throw new ArgumentException("Unsupported scheme");
            }

            #endregion

            #region Eat up 3 slashes after file: to find the start of the path.

            #endregion

            int slashIndex = RedisReference.UriScheme.Length;
            for (int i = 3; i > 0; i--)
            {
                slashIndex = uri.IndexOf('/', slashIndex + 1);
                if (slashIndex < 0)
                {
                    // FUTURE: Relative paths?
                    return null;
                    //throw new ArgumentException("Must be in the format scheme:///path");
                }
            }

            string path = uri.Substring(slashIndex + 1);
            return new RedisReference(path);
        }

        #region Persistence

        //public void Set(IReference reference, object obj) => DefaultOBase.Set(reference, obj);

        public override IOBase TryGetOBase(IReference reference)
        {
            throw new NotImplementedException();
            //if (IsValid(reference)) return this.DefaultOBase;
            //return null;
        }

        #endregion
    }
}


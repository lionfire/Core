using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Postgres
{
    public class PostgresOBus : OBusBase, IDefaultOBaseProvider
    {
        public override IOBase DefaultOBase => PostgresOBase.DefaultInstance;

        public override IEnumerable<Type> ReferenceTypes
        {
            get
            {
                yield return typeof(PostgresReference);
            }
        }
        
        public override IEnumerable<Type> HandleTypes
        {
            get
            {
                yield return typeof(OBusHandle<>);
            }
        }

        public override IEnumerable<string> UriSchemes => PostgresReference.UriSchemes;

        //public bool IsValid(IReference reference) => reference.GetOBases().Any();

        //public IOBase GetOBaseForConnectionString(string connectionString)
        //{
        //    if (connectionString != null)
        //    {
        //        throw new ArgumentException("connectionString must be null");
        //    }

        //    // For now, there is only one instance, representing the local file system.
        //    return PostgresOBase.Instance;
        //}


        public override IReference TryGetReference(string uri, bool strictMode)
        {
            int colonIndex = uri.IndexOf(':');
            if (colonIndex < 0)
            {
                return null;
                //throw new ArgumentException("Scheme missing");
            }

            #region // TODO: Verify scheme is supported!

            if (!uri.StartsWith(PostgresReference.UriScheme))
            {
                return null;
                //throw new ArgumentException("Unsupported scheme");
            }

            #endregion

            #region Eat up 3 slashes after file: to find the start of the path.

            #endregion

            int slashIndex = PostgresReference.UriScheme.Length;
            for (int i = 3; i > 0; i--)
            {
                slashIndex = uri.IndexOf('/', slashIndex+1);
                if (slashIndex < 0)
                {
                    // FUTURE: Relative paths?
                    return null;
                    //throw new ArgumentException("Must be in the format scheme:///path");
                }
            }

            string path = uri.Substring(slashIndex+1);
            return new PostgresReference(path);
        }

        #region Persistence

        //public void Set(IReference reference, object obj) => DefaultOBase.Set(reference, obj);

        public override IOBase TryGetOBase(IReference reference)
        {
            if (IsValid(reference)) return this.DefaultOBase;
            return null;
        }

        #endregion
    }
}


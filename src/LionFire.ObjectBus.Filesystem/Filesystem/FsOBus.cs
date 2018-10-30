using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.ObjectBus.Filesystem
{
    public class FsOBus : OBusBase, IDefaultOBaseProvider
    {
        //#region (Static) Singleton

        //public static FilesystemOBus Instance => ManualSingleton<FilesystemOBus>.GuaranteedInstance;

        //#endregion

        public override IOBase SingleOBase => FsOBase.Instance;

        public override IEnumerable<Type> ReferenceTypes
        {
            get
            {
                yield return typeof(LocalFileReference);
            }
        }
        
        public override IEnumerable<Type> HandleTypes
        {
            get
            {
                yield return typeof(OBusHandle<>);
            }
        }

        public override IEnumerable<string> UriSchemes => LocalFileReference.UriSchemes;

        //public override H<T> GetHandle<T>(IReference reference, T handleObject = default(T)) => throw new NotImplementedException();
        //public override R<T> GetReadHandle<T>(IReference reference) => throw new NotImplementedException();
        //public override bool IsCompatibleWith(IReference obj) => throw new NotImplementedException();

        //public bool IsValid(IReference reference) => reference.GetOBases().Any();
        //public override bool IsValid(IReference reference) => throw new NotImplementedException();


        //public IEnumerable<IOBase> OBases
        //{
        //    get { yield return FsOBase.Instance; }
        //}
        //public IOBase GetOBaseForConnectionString(string connectionString)
        //{
        //    if (connectionString != null)
        //    {
        //        throw new ArgumentException("connectionString must be null");
        //    }

        //    // For now, there is only one instance, representing the local file system.
        //    return FsOBase.Instance;
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

            if (!uri.StartsWith(LocalFileReference.UriScheme))
            {
                return null;
                //throw new ArgumentException("Unsupported scheme");
            }

            #endregion

            #region Eat up 3 slashes after file: to find the start of the path.

            #endregion
            int slashIndex = LocalFileReference.UriScheme.Length;
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
            return new LocalFileReference(path);
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


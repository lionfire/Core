using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus.Filesystem
{

    public class FsOBaseProvider : IOBaseProvider
    {
        public static FsOBaseProvider Instance { get { return Singleton<FsOBaseProvider>.Instance; } }

        #region URI Scheme

        public string[] UriSchemes
        {
            get { return FileReference.UriSchemes; }
        }
        public string UriSchemeDefault
        {
            get { return FileReference.UriScheme; }
        }

        #endregion

        public IEnumerable<IOBase> GetOBases(IReference reference)
        {
            FileReference fileReference = reference as FileReference;

            if (fileReference != null)
            {
                //if (fileReference.IsValid())
                {
                    yield return FsOBase.Instance;
                }
            }
            yield break;
        }
        
        public IEnumerable<IOBase> OBases
        {
            get { yield return FsOBase.Instance; }
        }

        public IOBase GetOBase(string connectionString)
        {
            if (connectionString != null)
            {
                throw new ArgumentException("connectionString must be null");
            }

            // For now, there is only one instance, representing the local file system.
            return FsOBase.Instance;
        }


        public IOBase DefaultOBase
        {
            get { return FsOBase.Instance; }
        }

        public IReference ToReference(string uri)
        {
            int colonIndex = uri.IndexOf(':');
            if (colonIndex < 0)
            {
                throw new ArgumentException("Scheme missing");
            }

            #region // TODO: Verify scheme is supported!
            if (!uri.StartsWith(FileReference.UriScheme)) throw new ArgumentException("Unsupported scheme");
            #endregion

            int slashIndex = FileReference.UriScheme.Length;
            for (int i = 3; i > 0; i--)
            {
                slashIndex = uri.IndexOf('/', slashIndex);
                if (slashIndex < 0) throw new ArgumentException("Must be in the format scheme:///path");
            }

            string path = uri.Substring(slashIndex); // UNTESTED
            return new FileReference(path);
        }

        #region Persistence

        public void Set(IReference reference, object obj)
        {
            DefaultOBase.Set(reference, obj);
        }

        #endregion
    }
}

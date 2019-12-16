using LionFire.CouchDB;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.CouchDB
{
    public class CouchPersisterOptions
    {

    }


    //public class ConnectionManagerPersisterBase<TOptions, TConnectionManager>
    //    where TConnectionManager : IConnectionManager
    //{

    //}

    public class CouchReference : FileReferenceBase<CouchReference>
    {

        #region Scheme

        public override string UriPrefixDefault => throw new NotImplementedException();

        public override string UriSchemeColon => "couch://";

        public override string UriScheme => "couch";

        public override string Scheme => "couch";

        #endregion

        #region Construction

#error NEXT

        #endregion


        #region Persister

        [SetOnce]
        public override string Persister
        {
            get => persister;
            set
            {
                if (persister == value) return;
                if (persister != default) throw new AlreadySetException();
                persister = value;
            }
        }
        private string persister;

        #endregion

        #region Couch-specific terms

        public string Id => Path;

        #endregion
    }

    public class CouchFileReference : FileReferenceBase<CouchReference>
    {

    }

    public class CouchPersister : PersisterBase<CouchPersisterOptions>
                , IPersister<CouchReference>
        //, IFilesystemPersistence<TReference, TPersistenceOptions>
        //, IWriter<string>
        //, IReader<string>
        //where TReference : IReference
        //where TPersistenceOptions : FilesystemPersisterOptions
    {
        protected CouchDBConnectionManager ConnectionManager { get; }

        public CouchPersister(CouchDBConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public Task<IPersistenceResult> Exists(IReferencable<CouchReference> referencable) => throw new NotImplementedException();
        public Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<CouchReference> referencable) => throw new NotImplementedException();
        public Task<IPersistenceResult> Create<TValue>(IReferencable<CouchReference> referencable, TValue value) => throw new NotImplementedException();
        public Task<IPersistenceResult> Update<TValue>(IReferencable<CouchReference> referencable, TValue value) => throw new NotImplementedException();
        public Task<IPersistenceResult> Upsert<TValue>(IReferencable<CouchReference> referencable, TValue value) => throw new NotImplementedException();
        public Task<IPersistenceResult> Delete(IReferencable<CouchReference> referencable) => throw new NotImplementedException();
    }
}

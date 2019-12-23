using LionFire.Referencing;
using System;

namespace LionFire.Persistence.CouchDB
{

    public class CouchDBReference : CouchDBReferenceBase<CouchDBReference>, IReference, ICouchDBReference
    {

        #region Construction

        public CouchDBReference() { }
        public CouchDBReference(string id)
        {
            this.Id = id;
        }
        public CouchDBReference(string database, string id) : this(id)
        {
            this.Database = database;
        }
        public CouchDBReference(string provider, string database, string id) : this(database, id)
        {
            this.Persister = persister;
        }
        #endregion

        #region Persister

        public string Host => throw new NotImplementedException();

        public string Port => throw new NotImplementedException();

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

        #region Id

        [SetOnce]
        public override string Id
        {
            get => id;
            set
            {
                if (id == value) return;
                if (id != default) throw new AlreadySetException();
                id = value;
            }
        }

        private string id;

        #endregion

        #endregion


    }
}

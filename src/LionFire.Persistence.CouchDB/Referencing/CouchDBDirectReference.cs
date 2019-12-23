using LionFire.Referencing;

namespace LionFire.Persistence.CouchDB
{
    public class CouchDBDirectReference : CouchDBReferenceBase<CouchDBDirectReference>, IReference, ICouchDBReference
    {
        #region Construction

        public CouchDBDirectReference() { }
        public CouchDBDirectReference(string id)
        {
            this.Id = id;
        }
        public CouchDBDirectReference(string database, string id) : this(id)
        {
            this.Database = database;
        }
        public CouchDBDirectReference(string host, string database, string id) : this(database, id)
        {
            this.Host = host;
        }
        public CouchDBDirectReference(string host, string port, string database, string id) : this(host, database, id)
        {
            this.Port = port;
        }

        #endregion

        public string Host { get; private set; }

        public string Port { get; private set; }

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

    }
}

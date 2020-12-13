using LionFire.Referencing;
using System;

namespace LionFire.Persistence.CouchDB
{
    public abstract class CouchDBReferenceBase<TConcrete> : ReferenceBaseBase<TConcrete>
        where TConcrete : ReferenceBaseBase<TConcrete>, IReference
    {
        #region Scheme

        public string UriPrefixDefault => throw new NotImplementedException();

        public string UriSchemeColon => "couch://";

        public string UriScheme => "couch";

        public override string Scheme => "couch";

        #region Database

        [SetOnce]
        public string Database
        {
            get => database;
            set
            {
                if (database == value) return;
                if (database != default) throw new AlreadySetException();
                database = value;
            }
        }
        private string database;

        #endregion


        #endregion

        public override string Key { get => Path; protected set => Path = value; }

        public abstract string Id
        {
            get; set;
        }

        public override string Path
        {
            get
            {
                if (database != null) return $"/{database}/{Id}";
                else return Id;
            }
            protected set => throw new NotImplementedException("TODO: parse");
        }

        public bool IsCompatibleWith(string obj) => throw new NotImplementedException();
    }
}

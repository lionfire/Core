using LionFire.Serialization;
using System;
using System.IO;

namespace LionFire.Persistence.Persisters
{
    public class PersisterBase<TOptions>
    {
        public ISerializationProvider SerializationProvider { get; set; }

        #region PersistenceOptions

        public TOptions PersistenceOptions
        {
            get => persistenceOptions;
            set
            {
                if (persistenceOptions != null) throw new AlreadySetException();
                persistenceOptions = value;
            }
        }
        private TOptions persistenceOptions;

        #endregion

        public virtual bool AllowAutoRetryForThisException(Exception e)
        {
            return !(
                e is SerializationException
                || e is FileNotFoundException
                );
        }

    }
}

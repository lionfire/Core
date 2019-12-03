using LionFire.Serialization;
using System;
using System.IO;

namespace LionFire.Persistence // MOVE
{
    public class PersisterBase<TOptions>
    {
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

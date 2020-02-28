using LionFire.Serialization;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace LionFire.Persistence.Persisters
{
    public class PersisterBase<TOptions>
    {

        #region Construction

        public PersisterBase(SerializationOptions serializationOptions)
        {
            SerializationOptions = serializationOptions;
        }

        #endregion

        #region SerializationProvider

        [SetOnce]
        public ISerializationProvider SerializationProvider
        {
            get => serializationProvider;
            set
            {
                if (serializationProvider == value) return;
                if (serializationProvider != default) throw new AlreadySetException();
                serializationProvider = value;
            }
        }
        private ISerializationProvider serializationProvider;

        #endregion

        // SetOnce?
        public SerializationOptions SerializationOptions { get; set; }

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

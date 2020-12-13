using LionFire.Serialization;
using Microsoft.Extensions.Options;

namespace LionFire.Persistence.Persisters
{
    /// <summary>
    /// Base for Persisters that must serialize .NET objects before writing them to storage.  Uses a ISerializationProvider.
    /// Object databases and storage mechanisms that can themselves persist .NET objects should not use this, but rather PersisterBase.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class SerializingPersisterBase<TOptions> : PersisterBase<TOptions>, ISerializingPersister
    {
        // SetOnce?
        public SerializationOptions SerializationOptions { get; set; }

        #region Construction

        public SerializingPersisterBase(SerializationOptions serializationOptions)
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
    }
}

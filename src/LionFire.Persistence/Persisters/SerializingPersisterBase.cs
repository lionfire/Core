using LionFire.Serialization;
using Microsoft.Extensions.Options;
//using static LionFire.Persistence.Filesystemlike.VirtualFilesystemPersisterBase<TReference, TPersistenceOptions>;

namespace LionFire.Persistence.Persisters;

/// <summary>
/// Base for Persisters that must serialize .NET objects before writing them to storage.  Uses a ISerializationProvider.
/// Object databases and storage mechanisms that can themselves persist .NET objects should not use this, but rather PersisterBase.
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public class SerializingPersisterBase<TOptions> : PersisterBase<TOptions>, ISerializingPersister
    where TOptions : PersistenceOptions
{
    // SetOnce?
    public SerializationOptions SerializationOptions { get; set; }

    #region Construction

    public SerializingPersisterBase(SerializationOptions serializationOptions, PersisterEvents? persisterEvents = null) : base(persisterEvents)
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


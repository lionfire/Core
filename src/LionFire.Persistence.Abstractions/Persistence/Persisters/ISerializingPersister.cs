#nullable enable
using LionFire.Serialization;

namespace LionFire.Persistence.Persisters;

public interface ISerializingPersister : IPersister
{
    ISerializationProvider SerializationProvider { get; }
}

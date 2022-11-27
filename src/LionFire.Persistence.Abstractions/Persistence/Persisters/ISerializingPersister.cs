#nullable enable
using LionFire.Serialization;

namespace LionFire.Persistence.Persisters;

public interface ISerializingPersister
{
    ISerializationProvider SerializationProvider { get; }
}

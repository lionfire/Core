#nullable enable
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Persisters
{
    //public interface IPersister : IPersister<IReference>
    //{
    //}
    public interface IPersister<in TReference> : IReadPersister<TReference>, IWritePersister<TReference>, IListPersister<TReference>
        where TReference : IReference
    {
    }

    public interface ISerializingPersister
    {
        ISerializationProvider SerializationProvider { get; }
    }

    //public enum PersisterOptionFlags
    //{
    //    EtagsSupported = 1 << 1,
    //    EtagsRequired = 1 << 1,
    //}
    //public struct PersisterOptions
    //{
    //}

    //public enum PersistenceOperationType
    //{
    //    Unspecified = 0,
    //    Create = 1 << 0,
    //    Retrieve = 1 << 1,
    //    Exists = 1 << 2,
    //    Update = 1 << 3,
    //    Upsert = 1 << 4,
    //    Delete = 1 << 5,

    //    /// <summary>
    //    /// Retrieves last eTag
    //    /// </summary>
    //    IsChanged = 1 << 10,

    //    List = 1 << 16,
    //    Add = 1 << 17,
    //    Remove = 1 << 18,
    //}

}

#nullable enable
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Persisters;

public interface IPersister
{
}
public interface IPersister<in TReference> : IPersister, IReadPersister<TReference>, IWritePersister<TReference>, IListPersister<TReference>
    where TReference : IReference
{
}

// ENH - enhancements for Persisters

//public enum PersisterCapabilities
//{
//    Unspecified = 0,
//    ReadBytes = 1 << 0,
//    WriteBytes = 1 << 1,

//    ReadStream = 1 << 3,
//    WriteStream = 1 << 4,

//    Deserialize = 1 << 5,
//    Serialize = 1 << 6,
//    DetectType = 1 << 7, // For List, returns types of list items. For Retrieve, instantiates correct type of C# object
//}

//public enum PersisterNativeTypes
//{
//    Unspecified = 0,
//    ByteArray = 1 << 0,
//    String = 1 << 1,
//    TypedObject = 1 << 2,
//    Stream = 1 << 3,

//}

//public enum PersisterOptionFlags
//{
//    EtagsSupported = 1 << 1,
//    EtagsRequired = 1 << 1,
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

using ICSharpCode.SharpZipLib.Zip;
using LionFire.Persistence;
using LionFire.Serialization;

namespace LionFire.Persisters.SharpZipLib.Tests.Deserialize;

public class SharpZipLibSerializer : SerializerBase<SharpZipLibSerializer>
{
    public override bool SupportsDeserializedType(Type type) => type == typeof(ZipFile);
    public override bool SupportsSerializedType(Type type) => type == typeof(byte[]) || type == typeof(Stream);

    public override SerializationFormat DefaultFormat => defaultFormat;
    static readonly SerializationFormat defaultFormat = new SerializationFormat("zip");

    public override SerializationFlags SupportedCapabilities =>
        SerializationFlags.Binary
        | SerializationFlags.Compress
        | SerializationFlags.Decompress
        | SerializationFlags.Serialize
        | SerializationFlags.Deserialize
        ;

    public override DeserializationResult<T> ToObject<T>(byte[] bytes, Lazy<PersistenceOperation>? operation = null, PersistenceContext? context = null)
    {
        if (typeof(T) != typeof(ZipFile)) { return DeserializationResult<T>.NotSupported; }

        var ms = new MemoryStream(bytes);
        var zipFile = new ZipFile(ms);

        return (DeserializationResult<T>)(object)new DeserializationResult<ZipFile>(zipFile);
    }

    public override DeserializationResult<T> ToObject<T>(Stream stream, Lazy<PersistenceOperation>? operation = null, PersistenceContext? context = null)
    {
        if (typeof(T) != typeof(ZipFile)) { return DeserializationResult<T>.NotSupported; }
        var zipFile = new ZipFile(stream);

        return (DeserializationResult<T>)(object)new DeserializationResult<ZipFile>(zipFile);
    }
}
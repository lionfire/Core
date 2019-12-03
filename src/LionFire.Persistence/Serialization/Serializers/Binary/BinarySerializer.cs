namespace LionFire.Serialization
{
    public class BinarySerializer : BytesSerializerBase<BinarySerializer>
    {
        public override SerializationFormat DefaultFormat => new SerializationFormat("bin", "Binary", "binary/octet-stream", "application/octet-stream") { };
    }
}
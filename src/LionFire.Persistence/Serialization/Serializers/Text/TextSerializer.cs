namespace LionFire.Serialization
{
    public class TextSerializer : StringSerializerBase<TextSerializer>
    {
        public override SerializationFormat DefaultFormat => new SerializationFormat("txt", "Text", "text/plain") { };
    }
}
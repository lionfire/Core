namespace LionFire.Serialization.Contexts

{
    public enum DeserializationStrategy
    {
        Unspecified,
        FileName = 1 << 0,
        MimeType = 1 << 1,
        Headers = 1 << 2,
        Detect = 1 << 3,
    }
}

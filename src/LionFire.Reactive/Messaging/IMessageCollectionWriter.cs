namespace LionFire.Messaging
{
    public interface IMessageCollectionWriter
    {
        void Add(MessageEnvelope envelope);
    }
}

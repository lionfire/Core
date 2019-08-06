namespace LionFire.Serialization
{
    /// <summary>
    /// One of potentially several serialization services that manage multiple strategies.
    /// For one master service, use ISerializationProvider.  TODO: Swap the names of these
    /// </summary>
    public interface ISerializationService : IHasSerializationStrategies
    {
    }
}

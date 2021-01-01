namespace LionFire.Referencing
{
    public interface IReferencableAsValueType<out TValue> : IReferencable  // REVIEW
    {
        new IReference<TValue> Reference { get; }
    }
}

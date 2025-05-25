namespace LionFire.Referencing
{
    public interface IReferenceableAsValueType<out TValue> : IReferenceable  // REVIEW
    {
        new IReference<TValue> Reference { get; }
    }
}

// REVIEW - UNUSED? TODEPRECATE ?
namespace LionFire.Referencing
{
    public struct TypedReference<T> : IReference<T>
    {
        public TypedReference(IReference reference)
        {
            Reference = reference;
        }
        public IReference Reference { get; private set; }
    }
}

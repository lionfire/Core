#nullable enable

namespace LionFire.Referencing
{
    


    //public struct ReferenceWrapper : IReferenceable, IReferenceable<IReference>
    //{
    //    public IReference Reference { get; }

    //    public ReferenceWrapper(IReference reference) { Reference = reference; }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is IReferenceable referencable)
    //        {
    //            return Reference.Equals(referencable.Reference);
    //        }
    //        if (obj is IReference reference) // Not a good idea?
    //        {
    //            return Reference.Equals(reference);
    //        }
    //        return false;
    //    }

    //    public override int GetHashCode() => Reference.GetHashCode();

    //    public static bool operator ==(ReferenceWrapper left, ReferenceWrapper right) => left.Reference.Equals(right.Reference);

    //    public static bool operator !=(ReferenceWrapper left, ReferenceWrapper right) => !(left.Reference == right.Reference);
    //}
}

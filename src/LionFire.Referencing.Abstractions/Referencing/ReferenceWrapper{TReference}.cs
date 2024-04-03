#nullable enable

namespace LionFire.Referencing
{
    public struct ReferenceWrapper2<TReference> : IReferencable<TReference>
        where TReference : IReference
    {
        public TReference Reference { get; }
        IReference IReferencable.Reference => Reference;
        //IReference IReferencable<IReference>.Reference => Reference;

        public ReferenceWrapper2(TReference reference) { Reference = reference; }

        public override bool Equals(object? obj)
        {
            if (obj is IReferencable<TReference> referencable)
            {
                return Reference.Equals(referencable.Reference);
            }
            //if(obj is IReference reference) // Not a good idea?
            //{
            //    return Reference.Equals(reference);
            //}
            return false;
        }

        public override int GetHashCode() => Reference.GetHashCode();

        public static bool operator ==(ReferenceWrapper2<TReference> left, ReferenceWrapper2<TReference> right) => left.Reference.Equals(right.Reference);

        public static bool operator !=(ReferenceWrapper2<TReference> left, ReferenceWrapper2<TReference> right) => !(left.Reference.Equals(right.Reference));

        public override string ToString() => Reference?.ToString() ?? "(null)";
    }

    //public struct ReferenceWrapper<TReference> : IReferencable<TReference>, IReferencable
    //    where TReference : IReference
    //{
    //    public TReference Reference { get; }
    //    IReference IReferencable.Reference => Reference;

    //    public ReferenceWrapper(TReference reference) { Reference = reference; }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is IReferencable referencable)
    //        {
    //            return Reference.Equals(referencable.Reference);
    //        }
    //        //if (obj is IReference reference) // Not a good idea?
    //        //{
    //        //    return Reference.Equals(reference);
    //        //}
    //        return false;
    //    }

    //    public override int GetHashCode() => Reference.GetHashCode();

    //    public static bool operator ==(ReferenceWrapper<TReference> left, ReferenceWrapper<TReference> right) => left.Reference.Equals(right.Reference);

    //    public static bool operator !=(ReferenceWrapper<TReference> left, ReferenceWrapper<TReference> right) => !(left == right);
    //}
}

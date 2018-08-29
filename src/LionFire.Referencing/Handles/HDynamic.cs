using System;

namespace LionFire.Referencing
{
    public class HDynamic<ObjectType> : RDynamic<ObjectType>, H<ObjectType>
        where ObjectType : class
    {
        #region Construction

        public HDynamic() { }
        public HDynamic(ObjectType obj) : base(obj) { }
        public HDynamic(IReference reference, ObjectType obj) : base(reference, obj)
        {
        }
        public HDynamic(IReference reference) : base(reference)
        {
        }

        #endregion

        public void MarkDeleted() => throw new NotImplementedException();
    }
}

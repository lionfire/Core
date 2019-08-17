#if FUTURE 
using System;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public class HDynamic<ObjectType> : RDynamic<ObjectType>, H<ObjectType>
        where ObjectType : class
    {
        #region Construction

        public HDynamic() { }
        public HDynamic(IReference reference, ObjectType obj = null) : base(reference, obj)
        {
        }

        #endregion

        public void MarkDeleted() => throw new NotImplementedException();
        public Task Save(object persistenceContext = null) => throw new NotImplementedException();
    }
}
#endif
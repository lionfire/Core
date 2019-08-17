#if false // REVIEW
using LionFire.Persistence;
using LionFire.Referencing.Persistence;
using System;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public abstract class HRetrieveInfoBase<ObjectType> : RBase<ObjectType>, IHasObjectRetrievalInfo<ObjectType>
    where ObjectType : class
    {
        #region Construction

        public HRetrieveInfoBase()
        {
        }

        public HRetrieveInfoBase(IReference reference, ObjectType obj = null) : base(reference, obj) { }
        
        #endregion
        
        public RetrieveResult<ObjectType>? ResolveHandleResult
        {
            get;
            protected set;
        }

        public abstract Task<RetrieveResult<ObjectType>> TryRetrieveObjectWithInfo();
    }
}
#endif
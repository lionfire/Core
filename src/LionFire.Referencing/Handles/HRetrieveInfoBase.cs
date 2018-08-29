using LionFire.Referencing.Resolution;
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

        public HRetrieveInfoBase(ObjectType obj) : base(obj) { }
        
        #endregion
        
        public ResolveHandleResult<ObjectType>? ResolveHandleResult
        {
            get;
            protected set;
        }

        public abstract Task<ResolveHandleResult<ObjectType>> TryResolveObjectWithInfo();
    }
}

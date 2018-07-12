using LionFire.Referencing.Resolution;
using System;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    public abstract class HRetrieveInfoBase<ObjectType> : HBase<ObjectType>, IHasObjectRetrievalInfo<ObjectType>
    where ObjectType : class
    {
        public HRetrieveInfoBase()
        {
            throw new NotImplementedException("Needs more thought.  Not sure I want to force EffectiveReferenceResolver on derived classes.  May still want derived classes to be able to implement their own TryResovleObject method.");
        }

        public ResolveHandleResult<ObjectType>? ResolveHandleResult
        {
            get;
            protected set;
        }

        public abstract Task<ResolveHandleResult<ObjectType>> TryResolveObjectWithInfo();
    }
}

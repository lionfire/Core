//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
//using LionFire.Input; REVIEW
//using LionFire.Extensions.DefaultValues; REVIEW

namespace LionFire.ObjectBus
{
    // TODO: Use IOC, extension methods here instead of this class
    public class OBusHandleBase<ObjectType> : HandleBase<ObjectType>
        where ObjectType : class
    {
        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            ObjectType result;

#if !AOT
            if (typeof(ObjectType) != typeof(object))
            {
                result = OBus.TryGetAs<ObjectType>(this.Reference, RetrieveInfo);
            }
            else
#endif
            {
                result = (ObjectType)OBus.TryGet(this.Reference, RetrieveInfo);
            }

            //T obj = OBus.TryGetAs<T>(this.Reference);

            if (result == null)
            {
#if TRACE_LOAD_FAIL
                lFailLoad.Trace("Failed to retrieve " + this.Reference);
#endif
            }
            else
            {
#if DEBUG_LOAD
                lLoad.Debug("HandleBase2 Retrieved " + this.Reference);
#endif
            }

            OnRetrieved(result);

            return result != null;
        }
    }
}

using System;
using System.Runtime.CompilerServices;
//using LionFire.Instantiating;

namespace LionFire.Machine.Sentinel.Controllers
{
    public class DataIdTracker
    {
        ConditionalWeakTable<object, DataId> guids = new ConditionalWeakTable<object, DataId>();

        public Guid? Get(object obj)
        {
            DataId result;
            if (guids.TryGetValue(obj, out result))
            {
                return result.Guid;
            }
            return null;
        }
        public void Set(object obj, DataId guid)
        {

        }
    }
}

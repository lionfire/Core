//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
//using LionFire.Input;
//using LionFire.Extensions.DefaultValues;

using LionFire.Persistence;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public static class OBusHandleExtensions
    {
        public static void Create(this H handle)
        {
            if(handle is INotifyCreating nc)
             {
                nc.OnCreating();
            }
            OBus.Create(handle.Reference, handle.Object); // Throws if already exists
            //OBus.CreateOrOverwrite(this.Reference, this.Object);

        }
    }
}

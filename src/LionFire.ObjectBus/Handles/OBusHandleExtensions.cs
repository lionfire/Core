//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
//using LionFire.Input;
//using LionFire.Extensions.DefaultValues;

namespace LionFire.ObjectBus
{
    public static class OBusHandleExtensions
    {
        public static void Create(this IHandle handle)
        {
            if(handle is INotifyCreating nc)
             {
                nc.OnCreating();
            }
            OBus.Create(this.Reference, this.Object); // Throws if already exists
            //OBus.CreateOrOverwrite(this.Reference, this.Object);

        }
    }
}

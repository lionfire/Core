//#define TRACE_GET
#define TRACE_GET_NOTFOUND

#if NET4
#define WEAKMETADATA // Experimental way to attach various info
#endif
//#define USE_READCACHE

namespace LionFire.ObjectBus
{
    public enum OBusOperations
    {
        Unspecified,
        Get,
        Set,
        Delete,
        GetChildren,
    }
}

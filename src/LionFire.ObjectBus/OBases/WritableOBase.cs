#define TRAGE_GET
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public abstract class WritableOBase<ReferenceType> : OBase<ReferenceType>, IOBase
        where ReferenceType : class, IReference
        //where HandleInterfaceType : IHandle
    {
    }
}

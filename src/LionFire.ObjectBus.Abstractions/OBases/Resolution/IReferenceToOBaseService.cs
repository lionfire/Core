using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public interface IReferenceToOBaseService 
    {
        (IOBus OBus, IOBase OBase) Resolve(IReference input);
        IOBus ResolveOBus(IReference input);
    }
}

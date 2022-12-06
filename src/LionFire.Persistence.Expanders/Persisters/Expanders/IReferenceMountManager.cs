#if FUTURE // maybe
using LionFire.Referencing;

namespace LionFire.Persisters.Expanders;

public interface IReferenceMountManager
{
    void Mount(IReference reference, ReferenceMountOptions referenceMountOptions);

    void Unmount(IReference reference);
}

public class ReferenceMountOptions
{
    public FileAccess FileAccess { get; set; } = FileAccess.Read;
    public TimeSpan? Timeout { get; set; }
}

#endif
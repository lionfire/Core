using LionFire.Referencing;

namespace LionFire.Vos.Mounts
{
    public interface ITMount
    {
        IVobReference MountPoint { get; set; }
        IMountOptions Options { get; set; }
        IReference Reference { get; set; }
    }
}
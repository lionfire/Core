using LionFire.Referencing;

namespace LionFire.Vos.Mounts
{
    public interface ITMount
    {
        IVobReference MountPoint { get; set; }
        IVobMountOptions Options { get; set; }
        IReference Reference { get; set; }
    }
}
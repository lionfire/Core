using LionFire.Referencing;

namespace LionFire.Vos.Mounts
{
    public interface IMount
    {

        IVob MountPoint { get; }
        IReference Target { get; }
        bool IsEnabled { get; set; }

        IVobMountOptions Options { get; }

        int VobDepth { get; }
    }
}

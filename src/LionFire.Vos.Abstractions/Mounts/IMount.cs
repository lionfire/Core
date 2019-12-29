using LionFire.Referencing;

namespace LionFire.Vos.Mounts
{
    public interface IMount
    {

        IVob MountPoint { get; }
        IReference Target { get; }

        IMountOptions Options { get; }

        int VobDepth { get; }
    }
}

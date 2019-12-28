using LionFire.Referencing;
using LionFire.Vos.Mounts;
using LionFire.Vos.Services;

namespace LionFire.Vos.ExtensionMethods
{
    public static class IVobMountExtensions
    {
        public static void Mount(this IVob vob, IReference target, MountOptions options = null)
        {
            vob.GetRequiredService<VobMounter>().Mount(vob, target, options);
        }
    }
}

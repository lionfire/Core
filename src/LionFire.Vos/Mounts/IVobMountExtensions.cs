using LionFire.Referencing;
using LionFire.Vos.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Mounts
{
    public static class IVobMountExtensions
    {
        public static IMount Mount(this IVob vob, IReference target, MountOptions options = null)
          => vob.GetRequiredService<VobMounter>().Mount(vob, target, options);
        public static IMount Mount(this IVob vob, IVob target, MountOptions options = null)
          => vob.GetRequiredService<VobMounter>().Mount(vob, target, options);

        public static IMount Mount(this IVob vob, ITMount tMount)
            => vob.GetRequiredService<VobMounter>().Mount(vob, tMount);

        public static int Unmount(this IVob vob, IReference target)
          => vob.GetRequiredService<VobMounter>().Unmount(vob, target);
    }
}

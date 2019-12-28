using LionFire.Referencing;
using LionFire.Vos.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos.Mounts
{

    public static class IVobMountExtensions
    {
        public static Mount Mount(this IVob vob, IReference target, MountOptions mountOptions = null)
        {
            vob.GetRequiredService<VobMounter>().Mount(vob, target, mountOptions);
            
        }
    }
}

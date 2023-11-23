﻿using LionFire.Referencing;

namespace LionFire.Vos.Mounts
{
    public interface IVobMounter
    {
        IMount Mount(IVob mountPoint, IReference target, IVobMountOptions options = null);
    }
}

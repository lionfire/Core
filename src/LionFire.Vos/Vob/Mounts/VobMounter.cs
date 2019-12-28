using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Vos.Mounts
{
    public class VobMounter : IVobMounter
    {
        public IMount Mount(IVob mountPoint, IReference target, MountOptions options = null)
        {
            throw new NotImplementedException("NEXT");

            //if (tMount.VobPath != this.Path)
            //{
            //    if (!LionPath.IsSameOrDescendantOf(this.Path, tMount.VobPath))
            //    {
            //        throw new VosException("Mount point must be this Vob or a descendant");
            //    }
            //    return Root[tMount.VobPath].Mount(tMount);
            //}

            //return GetVobNode().Mount(new Mount(this, tMount));

        }
    }
}

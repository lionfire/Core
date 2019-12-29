using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;
using LionFire.Vos.Internals;

namespace LionFire.Vos.Mounts
{
    public class VobMounter : IVobMounter
    {
        public IMount Mount(IVob mountPoint, TMount tMount)
        {
            var mount = new Mount(mountPoint, tMount);
            return DoMount(mountPoint, mount);
        }
        public IMount Mount(IVob mountPoint, IReference target, IMountOptions options = null)
        {
            var mount = new Mount(mountPoint, target, options);
            return DoMount(mountPoint, mount);
        }

        private IMount DoMount(IVob mountPoint, Mount mount)
        {
            var args = new CancelableReasonEventArgs<Mount>(mount);
            foreach(Action<CancelableReasonEventArgs<Mount>> del in Mounting.GetInvocationList())
            {
                del.Invoke(args);
                if (args.IsCanceled)
                {
                    throw new Exception($"Mount was canceled.  Reason: {(args.Reason == null ? "(none)" : args.Reason)}");
                }
            }
            var result = mountPoint.GetOrAddOwn<VobMounts>().Mount(mount);
            Mounted?.Invoke(mount);
            return result;
        }

        public event Action<CancelableReasonEventArgs<IMount>> Mounting;
        public event Action<IMount> Mounted;
    }
}

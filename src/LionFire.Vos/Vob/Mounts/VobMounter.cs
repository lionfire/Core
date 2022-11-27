using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Referencing;
using LionFire.Vos.Internals;
using LionFire.Vos.Services;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos.Mounts
{
    public class VobMounter : IVobMounter
    {
        public ILogger<VobMounter> Logger { get; }

        public VobMounter(ILogger<VobMounter> logger)
        {
            Logger = logger;
        }

        public IMount Mount(IVob mountPointOrParent, ITMount tMount)
        {
            Logger.LogInformation($"[mount] {tMount} ");
            IVob mountPoint = mountPointOrParent;
            if (mountPointOrParent.Reference != tMount.MountPoint)
            {
                if (!LionPath.IsDescendantOf(mountPointOrParent.Path, tMount.MountPoint.Path))
                {
                    throw new ArgumentException($"{nameof(tMount)}.{nameof(tMount.MountPoint)} must be same or a descendant of {nameof(mountPointOrParent)}.{nameof(mountPointOrParent.Path)}");
                }
                mountPoint = mountPointOrParent.Root[tMount.MountPoint.Path]; // TODO: Add VobReference overload to Vob[] accessor?
            }
            var mount = new Mount(mountPoint, tMount);
            return DoMount(mount);
        }
        public IMount Mount(IVob mountPoint, IVob target, IVobMountOptions options = null)
        {
            var mount = new Mount(mountPoint, target, options);
            return DoMount(mount);
        }
        public IMount Mount(IVob mountPoint, IReference target, IVobMountOptions options = null)
        {
            var mount = new Mount(mountPoint, target, options);
            return DoMount(mount);
        }

        protected IMount DoMount(Mount mount)
        {
            IVob mountPoint = mount.MountPoint;
            var args = new CancelableReasonEventArgs<Mount>(mount);
            var ev = Mounting;
            if (ev != null)
            {
                foreach (Action<CancelableReasonEventArgs<Mount>> del in ev.GetInvocationList())
                {
                    del.Invoke(args);
                    if (args.IsCanceled)
                    {
                        throw new Exception($"Mount was canceled.  Reason: {(args.Reason == null ? "(none)" : args.Reason)}");
                    }
                }
            }
            var result = mountPoint.GetOrAddOwn<VobMounts>().Mount(mount);
            Mounted?.Invoke(mount);
            return result;
        }

        public event Action<CancelableReasonEventArgs<IMount>> Mounting;
        public event Action<IMount> Mounted;

        public int Unmount(IVob mountPoint, IReference target)
            => mountPoint.GetOrAddOwn<VobMounts>().Unmount(target);
    }
}

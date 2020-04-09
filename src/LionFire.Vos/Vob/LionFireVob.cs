#if UNUSED
using LionFire.Referencing;
using LionFire.Vos.Mounts;

namespace LionFire.Vos
{
    public class LionFireVob : Vob
    {
        public LionFireVob(Vob parent, string name) : base(parent, name)
        {
            Path = LionPath.GetTrimmedAbsolutePath(LionPath.Combine((parent == null ? "" : parent.Path), name));

        }

        public VobMounts Mounts { get; set; }

        #region Caches

        public override string Path { get; }

        #region Root

        public override RootVob Root
        {
            get
            {
                if (root == null)
                {
                    IVob vob = this;
                    while (vob.Parent != null && vob.Parent.Path != VosConstants.VobRootsRoot) { vob = vob.Parent; }
                    root = vob as RootVob;
                }
                return root;
            }
        }
        private RootVob root;

        #endregion

        public VobMountCache MountCache { get; } = new VobMountCache();

        #endregion

    }
}
#endif
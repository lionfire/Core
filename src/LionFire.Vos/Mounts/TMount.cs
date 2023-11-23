using LionFire.Referencing;
using LionFire.Instantiating;
using Microsoft.Extensions.Options;

namespace LionFire.Vos.Mounts
{
    // REVIEW - Consider renaming this to Mount, and Mount to MountState

    public class TMount : ITemplate<Mount>, ITMount
    {
        public TMount() { }
        public TMount(IVobReference vobReference, IReference reference, IVobMountOptions options = null)
        {
            MountPoint = vobReference;
            Reference = reference;
            Options = options;
        }

        public IVobReference MountPoint { get; set; }

        public IReference Reference { get; set; }

        public IVobMountOptions Options { get; set; }

        public string ArrowSymbol
        {
            get
            {
                if (Options != null)
                {
                    if (Options.WritePriority.HasValue)
                    {
                        if (Options.IsExclusive)
                        {
                            return "==>>";
                        }
                        else
                        {
                            return "-->>";
                        }
                    }
                    else
                    {
                        if (Options.IsExclusive)
                        {
                            return "==>";
                        }
                        else
                        {
                            return "-->";
                        }
                    }
                }
                return "->";
            }
        }

        public override string ToString() => $"{{TMount {MountPoint} {ArrowSymbol} {Reference}}}";
    }
}

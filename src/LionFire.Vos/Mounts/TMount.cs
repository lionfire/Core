using LionFire.Referencing;
using LionFire.Instantiating;

namespace LionFire.Vos
{
    // REVIEW - Consider renaming this to Mount, and Mount to MountState

    public class TMount : ITemplate<Mount>
    {
        public TMount() { }
        public TMount(string vobPath, IReference reference, MountOptions options = null)
        {
            VobPath = vobPath;
            Reference = reference;
            Options = options;
        }

        public string VobPath { get; set; }

        public IReference Reference { get; set; }

        public MountOptions Options { get; set; }
    }
}

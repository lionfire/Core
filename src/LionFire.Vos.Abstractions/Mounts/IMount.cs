using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Vos.Mounts;

public interface IMount : IParented<IMount>
{

    IVob MountPoint { get; }
    IReference Target { get; }
    bool IsEnabled { get; set; }

    IVobMountOptions Options { get; }

    int VobDepth { get; }

    public IMount UpstreamMount => Options?.UpstreamMount;
}

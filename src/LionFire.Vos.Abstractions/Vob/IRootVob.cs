using LionFire.Ontology;
using Microsoft.Extensions.Hosting;

namespace LionFire.Vos
{
    public interface IRootVob : IVob
    {
        string RootName { get; }
        IVos RootManager { get; }

        VobRootOptions Options { get; }


    }

}

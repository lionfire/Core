#nullable enable
using LionFire.Ontology;

namespace LionFire.Vos
{
    public interface IRootManager : IHas<IRootManager>
    {
        IRootVob? Get(string? rootName = null);
    }


}

using LionFire.Ontology;

namespace LionFire.Vos
{
    public interface IRootVob : IVob 
    {
        IRootManager RootManager { get; }
    }

}

using LionFire.Ontology;

namespace LionFire.Vos
{
    public interface IRootVob : IVob 
    {
        string RootName { get; }
        IRootManager RootManager { get; }
    }

}

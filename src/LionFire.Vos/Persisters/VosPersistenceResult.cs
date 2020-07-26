using LionFire.Referencing;

namespace LionFire.Persistence.Persisters.Vos
{
    public class VosPersistenceResult : PersistenceResult
    {
        public IReference ResolvedVia { get; set; }
    }
}

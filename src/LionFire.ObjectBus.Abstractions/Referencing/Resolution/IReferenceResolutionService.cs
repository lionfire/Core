using System.Collections.Generic;

namespace LionFire.Persistence.Resolution
{
    public interface IReferenceResolutionService : IReferenceToReferenceResolver
    {
        IEnumerable<IReferenceResolutionStrategy> Strategies { get; }
    }

}

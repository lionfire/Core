using System.Collections.Generic;

namespace LionFire.Referencing.Persistence
{
    public interface IReferenceResolutionService : IReferenceToReferenceResolver
    {
        IEnumerable<IReferenceResolutionStrategy> Strategies { get; }
    }

}

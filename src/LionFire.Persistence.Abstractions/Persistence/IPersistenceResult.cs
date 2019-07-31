using System.Collections.Generic;

namespace LionFire.Persistence
{
    public interface IPersistenceResult
    {
        object Error { get; }

        int WriteCount { get; set; }

        PersistenceResultKind Kind { get; set;}

        IEnumerable<IPersistenceResult> Successes { get; set; }
        IEnumerable<IPersistenceResult> Failures { get; set; }
    }

}